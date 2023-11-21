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
        private void GenerateSyntaxPartials(ITypeSymbol recordSymbol) {
            var langName = Utils.getLangName(recordSymbol);
            var ownProperties = Utils.getOwnProperties(recordSymbol).ToList();
            var inheritedProperties = Utils.getInheritedProperties(recordSymbol).ToList();

            var compilationUnit = CompilationUnit()
                .AddMembers(GenerateSyntaxNamespace(recordSymbol)
                    .AddMembers(PublicSyntax(recordSymbol, langName.ToLower(), ownProperties, inheritedProperties))
                ).AddUsings(
                    UsingDirective(IdentifierName("System.Collections.Generic")),
                    UsingDirective(IdentifierName($"{ROOT_NAMESPACE}.Syntax")),
                    UsingDirective(IdentifierName("System.Linq"))
                );

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
                    .AddMembers(generateChildrenGetter(recordSymbol, inheritedAndOwnProperties));
            }

            // skip MapWith if it exists
            if (!recordSymbol.GetMembers("MapWith").Any() && !recordSymbol.IsAbstract) {
                recordDeclaration = recordDeclaration.AddMembers(generateMapper(recordSymbol, inheritedAndOwnProperties));
            }

            // generate internal syntax
            // if (!recordSymbol.GetMembers("Internal").Any()) {
            //     recordDeclaration = recordDeclaration.AddMembers(InternalSyntax(recordSymbol, langName, ownProperties,
            //         inheritedProperties));
            // }

            return Utils.wrapRecordInEnclosingClasses(recordDeclaration, recordSymbol, false);
        }

        private static RecordDeclarationSyntax InternalSyntax(ITypeSymbol recordSymbol, string langName,
            List<IPropertySymbol> ownProperties, List<IPropertySymbol> inheritedProperties) {
            var recordDeclaration = RecordDeclaration(Token(SyntaxKind.RecordKeyword), Identifier("Internal"))
                .AddBaseListTypes(SimpleBaseType(IdentifierName($"Syntax<{langName.ToLower()}>")))
                .AddModifiers(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.PartialKeyword))
                .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                // .AddMembers(generateToStringOverride().ToArray())
                .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));

            // if it doesn't have the override and is not abstract, generate childrenAccessor
            var inheritedAndOwnProperties = inheritedProperties.Concat(ownProperties).ToList();

            // skip children accessor if it exists
            if (!recordSymbol.GetMembers("ChildNodesAndTokens").Any() && !recordSymbol.IsAbstract)
                recordDeclaration =
                    recordDeclaration.AddMembers(generateChildrenGetter(recordSymbol, inheritedAndOwnProperties));

            // skip MapWith if it exists
            if (!recordSymbol.GetMembers("MapWith").Any() && !recordSymbol.IsAbstract)
                recordDeclaration = recordDeclaration.AddMembers(generateMapper(recordSymbol, inheritedAndOwnProperties));

            return recordDeclaration;
        }

        private static IEnumerable<MemberDeclarationSyntax> generateToStringOverride() {
            yield return MethodDeclaration(
                    PredefinedType(Token(SyntaxKind.StringKeyword)),
                    Identifier("ToString"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithExpressionBody(ArrowExpressionClause(InvocationExpression(MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    BaseExpression(),
                    IdentifierName("ToString")))))
                .WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken));
        }

        /// generate ChildNodesAndTokens accessor, handle correct nullability
        private static PropertyDeclarationSyntax generateChildrenGetter(
            ITypeSymbol recordSymbol,
            IEnumerable<IPropertySymbol> inheritedAndOwnProperties
        ) {
            var langName = Utils.getQualifiedNameParts(recordSymbol).Last();

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

        private static IEnumerable<LocalDeclarationStatementSyntax> mapperFieldDeclarations(
            IEnumerable<IPropertySymbol> inheritedAndOwnFields
        ) {
            foreach (var prop in inheritedAndOwnFields) {
                yield return LocalDeclarationStatement(
                    VariableDeclaration(IdentifierName(Identifier("var")))
                        .WithVariables(SingletonSeparatedList(
                            VariableDeclarator(
                                    Identifier(prop.Name)) // <-- this will fail for fields with `@` like `@default`
                                .WithInitializer(EqualsValueClause(InvocationExpression(
                                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("mapper"),
                                            IdentifierName("Map")))
                                    .WithArgumentList(ArgumentList(SingletonSeparatedList(
                                        Argument(CastExpression(
                                            IdentifierName("dynamic"),
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                ThisExpression(),
                                                IdentifierName(prop.Name))))))))))));
            }
        }

        private static InvocationExpressionSyntax referencesEqualCheck(IPropertySymbol property) {
            return InvocationExpression(IdentifierName("ReferenceEquals"))
                .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
                    new SyntaxNodeOrToken[]
                    {
                        Argument(IdentifierName(property.Name)),
                        Token(SyntaxKind.CommaToken),
                        Argument(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            ThisExpression(),
                            IdentifierName(property.Name)))
                    })));
        }

        private static ExpressionSyntax simpleEqualityCheck(IPropertySymbol property) {
            return BinaryExpression(SyntaxKind.EqualsExpression,
                IdentifierName(property.Name),
                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(),
                    IdentifierName(property.Name)));
        }

        private static StatementSyntax mapperReturnUnchanged(
            IEnumerable<IPropertySymbol> inheritedAndOwnFields
        ) {
            var checks = inheritedAndOwnFields.Select(simpleEqualityCheck).ToList();

            ExpressionSyntax testExpression = null;
            foreach (var check in checks) {
                if (testExpression is null)
                    testExpression = check;
                else
                    testExpression = BinaryExpression(SyntaxKind.LogicalAndExpression,
                        testExpression,
                        Token(SyntaxKind.AmpersandAmpersandToken).WithLeadingTrivia(ElasticLineFeed),
                        check);
            }

            if (testExpression is null)
                return ReturnStatement(ThisExpression());

            return IfStatement(testExpression, ReturnStatement(ThisExpression()));
        }

        private static ReturnStatementSyntax mapperObjectCreationExpression(
            ITypeSymbol recordSymbol,
            IEnumerable<IPropertySymbol> inheritedAndOwnProperties
        ) {
            return ReturnStatement(
                ObjectCreationExpression(IdentifierName(Utils.getTypeNameWithGenericArguments(recordSymbol)))
                    .WithInitializer(InitializerExpression(SyntaxKind.ObjectInitializerExpression,
                        SeparatedList<ExpressionSyntax>(
                            inheritedAndOwnProperties.Select(field =>
                                AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName(field.Name),
                                    IdentifierName(field.Name)))))));
        }

        private static IEnumerable<StatementSyntax> mapperFunctionStatements(
            ITypeSymbol recordSymbol,
            IEnumerable<IPropertySymbol> inheritedAndOwnProperties
        ) {
            foreach (var st in mapperFieldDeclarations(inheritedAndOwnProperties))
                yield return st;

            yield return mapperReturnUnchanged(inheritedAndOwnProperties);

            yield return mapperObjectCreationExpression(recordSymbol, inheritedAndOwnProperties);
        }

        private static MethodDeclarationSyntax generateMapper(
            ITypeSymbol recordSymbol,
            List<IPropertySymbol> inheritedAndOwnProperties
        ) {
            var langName = Utils.getQualifiedNameParts(recordSymbol).Last();
            return MethodDeclaration(
                    IdentifierName(Identifier($"Syntax<{langName.ToLower()}>")), Identifier("MapWith"))
                .WithModifiers(
                    TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier("mapper"))
                    .WithType(IdentifierName(Identifier($"Mapper<{langName.ToLower()}>"))))))
                .WithBody(Block(mapperFunctionStatements(recordSymbol, inheritedAndOwnProperties)));
        }
    }
}
