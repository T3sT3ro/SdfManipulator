using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace me.tooster.sdf.AST.Generators {
    public partial class AstSourceGenerator {
        private void GenerateSyntaxPartials(ITypeSymbol recordSymbol, SymbolSet ss) {
            var langName = ss.LangName;
            var ownProperties = Utils.getOwnProperties(recordSymbol).ToList();
            var inheritedProperties = Utils.getInheritedProperties(recordSymbol).ToList();
            var allUsings = new List<UsingDirectiveSyntax>() {
                UsingDirective(IdentifierName("System.Collections.Generic")),
                UsingDirective(IdentifierName($"{ROOT_NAMESPACE}.Syntax")),
                UsingDirective(IdentifierName("System.Linq")),
            };
                
            // TODO: deduplicate entries
            ss.Includes.TryGetValue(recordSymbol, out var usings);
            allUsings.AddRange(usings);
                        
            var compilationUnit = CompilationUnit()
                .AddMembers(GenerateSyntaxNamespace(recordSymbol)
                    .AddMembers(PublicSyntax(recordSymbol, langName.ToLower(), ownProperties, inheritedProperties))
                ).AddUsings(allUsings.ToArray())
                .WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)));

            context.AddSource(
                $"{string.Join(".", Utils.getQualifiedNameParts(recordSymbol).Reverse())}.g.cs",
                SourceText.From(compilationUnit.NormalizeWhitespace().SyntaxTree.ToString(), Encoding.UTF8)
            );
        }

        private RecordDeclarationSyntax PublicSyntax(
            ITypeSymbol recordSymbol,
            string langName,
            List<IPropertySymbol> ownProperties,
            List<IPropertySymbol> inheritedProperties
        ) {
            var recordDeclaration = RecordDeclaration(Token(SyntaxKind.RecordKeyword),
                    Identifier(Utils.getTypeNameWithGenericArguments(recordSymbol)))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword)))
                .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));

            // add base class to class if it's marked with [Syntax] (true based on filtering) and has empty derived list
            if (!recordSymbol.BaseType!.IsAbstract) {
                recordDeclaration = recordDeclaration.AddBaseListTypes(SimpleBaseType(
                    IdentifierName($"Syntax<{langName.ToLower()}>")));
            }

            // if it doesn't have the override and is not abstract, generate childrenAccessor
            var inheritedAndOwnProperties = inheritedProperties.Concat(ownProperties).ToList();

            // skip children accessor if it exists
            if (!(recordSymbol.GetMembers("ChildNodesAndTokens").Any() || recordSymbol.IsAbstract)) {
                recordDeclaration = recordDeclaration
                    .AddMembers(generateChildrenGetter(recordSymbol, inheritedAndOwnProperties, langName));
            }

            // skip MapWith if it exists
            // if (!recordSymbol.GetMembers("MapWith").Any() && !recordSymbol.IsAbstract) {
            //     recordDeclaration = recordDeclaration.AddMembers(
            //         generateMapperMethod(recordSymbol, inheritedAndOwnProperties, langName));
            // }

            // skip Accept and Accept<> if any of them exists
            // skip Accept<> if it exists (returning version)
            if (!recordSymbol.GetMembers("Accept").Any() && !recordSymbol.IsAbstract) {
                recordDeclaration =
                    recordDeclaration.AddMembers(GenerateVisitorAcceptor(recordSymbol, langName, false));
                recordDeclaration = recordDeclaration.AddMembers(GenerateVisitorAcceptor(recordSymbol, langName, true));
            }

            // generate internal syntax
            // if (!recordSymbol.GetMembers("Internal").Any()) {
            //     recordDeclaration = recordDeclaration.AddMembers(InternalSyntax(recordSymbol, langName, ownProperties,
            //         inheritedProperties));
            // }

            return Utils.wrapRecordInEnclosingClasses(recordDeclaration, recordSymbol, false);
        }

        /// generate ChildNodesAndTokens accessor, handle correct nullability
        private static PropertyDeclarationSyntax generateChildrenGetter(
            ITypeSymbol recordSymbol,
            IEnumerable<IPropertySymbol> inheritedAndOwnProperties,
            string langName
        ) {
            TypeSyntax itemType = IdentifierName(Identifier($"SyntaxOrToken<{langName.ToLower()}>"));

            var anyFieldNullable =
                inheritedAndOwnProperties.Any(f => f.Type.NullableAnnotation == NullableAnnotation.Annotated);


            var arrayCreationExpressionSyntax = ArrayCreationExpression(
                    ArrayType(anyFieldNullable ? NullableType(itemType) : itemType)
                        .WithRankSpecifiers(SingletonList(ArrayRankSpecifier(
                            SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression())))))
                .WithInitializer(InitializerExpression(
                    SyntaxKind.ArrayInitializerExpression,
                    SeparatedList<ExpressionSyntax>(
                        generateChildrenGetterMembers(inheritedAndOwnProperties))));

            return PropertyDeclaration(GenericName(Identifier("IReadOnlyList"))
                        .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(itemType))),
                    Identifier("ChildNodesAndTokens"))
                .WithModifiers(
                    TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        !anyFieldNullable
                            ? arrayCreationExpressionSyntax
                            : InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                        InvocationExpression(MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                arrayCreationExpressionSyntax,
                                                IdentifierName("Where")))
                                            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(
                                                SimpleLambdaExpression(Parameter(Identifier("n")))
                                                    .WithExpressionBody(IsPatternExpression(IdentifierName("n"),
                                                        UnaryPattern(ConstantPattern(LiteralExpression(
                                                            SyntaxKind.NullLiteralExpression))))))))),
                                        IdentifierName("Select")))
                                    .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(
                                        SimpleLambdaExpression(Parameter(Identifier("n")))
                                            .WithExpressionBody(PostfixUnaryExpression(
                                                SyntaxKind.SuppressNullableWarningExpression,
                                                IdentifierName("n"))))))),
                                IdentifierName("ToList")))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        /// generates contents of the ChildNodesAndTokens { a, b, c }; 
        private static IEnumerable<SyntaxNodeOrToken> generateChildrenGetterMembers(
            IEnumerable<IPropertySymbol> allProperties
        ) {
            foreach (var field in allProperties) {
                yield return IdentifierName(field.Name);
                yield return Token(SyntaxKind.CommaToken); // trailing comma is OK
            }
        }

        // internal override void Accept(AST.Visitor<langName> visitor, Anchor a) => ((Visitor)visitor).Visit(Anchor.New(this, a));
        public MethodDeclarationSyntax GenerateVisitorAcceptor(ITypeSymbol recordSymbol, string langName,
            bool returning) {
            return (MethodDeclarationSyntax)(returning
                ? ParseMemberDeclaration(
                    $"internal override R? Accept<R>(AST.Visitor<{langName}, R> visitor, Anchor a) where R : default => ((Visitor<R>)visitor).Visit(Anchor.New(this, a));")
                : ParseMemberDeclaration(
                    $"internal override void Accept(AST.Visitor<{langName}> visitor, Anchor a) => ((Visitor)visitor).Visit(Anchor.New(this, a));"))!;
        }
    }
}
