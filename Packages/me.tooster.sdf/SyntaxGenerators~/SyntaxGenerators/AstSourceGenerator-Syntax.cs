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
        private class UsingDirectiveComparer : IEqualityComparer<UsingDirectiveSyntax> {
            public bool Equals(UsingDirectiveSyntax x, UsingDirectiveSyntax y) =>
                x.Name.ToString() == y.Name.ToString();

            public int GetHashCode(UsingDirectiveSyntax obj) => obj.Name.ToString().GetHashCode();
        }

        private void GenerateSyntaxPartials(ITypeSymbol recordSymbol, SymbolSet ss) {
            var langName = ss.LangName;
            var ownProperties = recordSymbol.OwnProperties().Where(Utils.isPropertyCompatible).ToList();
            var inheritedProperties = recordSymbol.InheritedProperties().Where(Utils.isPropertyCompatible).ToList();
            var allUsings = new List<UsingDirectiveSyntax>()
            {
                UsingDirective(IdentifierName("System.Collections.Generic")),
                UsingDirective(IdentifierName($"{ROOT_NAMESPACE}.Syntax")),
                // UsingDirective(IdentifierName($"{ROOT_NAMESPACE}.{langName}.Tokens")),
                UsingDirective(IdentifierName("System.Linq")),
                UsingDirective(IdentifierName("System.Text")),
            };

            // TODO: deduplicate entries
            ss.Includes.TryGetValue(recordSymbol, out var usings);
            allUsings.AddRange(usings);

            var compilationUnit = CompilationUnit()
                .AddMembers(GenerateSyntaxNamespace(recordSymbol)
                    .AddMembers(PublicSyntax(recordSymbol, langName.ToLower(), ownProperties, inheritedProperties))
                ).AddUsings(allUsings.Distinct(new UsingDirectiveComparer()).ToArray())
                .WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)));

            // var name = recordSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted))
            context.AddSource(
                $"{string.Join(".", recordSymbol.qualifiedNameParts().Reverse())}.g.cs",
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
                    Identifier(recordSymbol.getTypeNameWithGenericArguments()))
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

            // skip Accept and Accept<> if any of them exists
            if (!recordSymbol.GetMembers("Accept").Any() && !recordSymbol.IsAbstract) {
                recordDeclaration =
                    recordDeclaration.AddMembers(GenerateVisitorAcceptor(recordSymbol, langName, false));
                recordDeclaration = recordDeclaration.AddMembers(GenerateVisitorAcceptor(recordSymbol, langName, true));
            }

            // skip toString if it exists
            if (Utils.generateToStringOverride(recordSymbol) is { } member) {
                recordDeclaration = recordDeclaration.AddMembers(member);
            }

            // generate internal syntax
            // if (!recordSymbol.GetMembers("Internal").Any()) {
            //     recordDeclaration = recordDeclaration.AddMembers(InternalSyntax(recordSymbol, langName, ownProperties,
            //         inheritedProperties));
            // }

            return recordDeclaration.wrapRecordInEnclosingClasses(recordSymbol, false);
        }

        /// generate ChildNodesAndTokens accessor, handle correct nullability
        private static MethodDeclarationSyntax generateChildrenGetter(
            ITypeSymbol recordSymbol,
            IEnumerable<IPropertySymbol> inheritedAndOwnProperties,
            string langName
        ) {
            var itemType = $"SyntaxOrToken<{langName.ToLower()}>";

            var anyFieldNullable =
                inheritedAndOwnProperties.Any(f => f.Type.NullableAnnotation == NullableAnnotation.Annotated);

            var method = $"public override IReadOnlyList<{itemType}> ChildNodesAndTokens() => new {itemType}[] {{"
              + string.Join(", ", inheritedAndOwnProperties.Select(p => p.Name))
              + $"}}" + (anyFieldNullable ? ".Where(c => c is not null).Select(c => c!).ToList();" : ";");

            return (MethodDeclarationSyntax)ParseMemberDeclaration(method)!;
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
                        $@"
                        internal override R? Accept<R>(AST.Visitor<{langName}, R> visitor, Anchor? parent) where R : default {{
                            if (visitor is Visitor<R> v) return v.Visit(Anchor.New(this, parent));
                            else return visitor.Visit(Anchor.New<{recordSymbol.languageAgnosticType()?.ToString() ?? $"Syntax<{langName}>"}>(this, parent));
                        }}")
                    : ParseMemberDeclaration(
                        $@"internal override void Accept(AST.Visitor<{langName}> visitor, Anchor? parent) {{
                            if (visitor is Visitor v) v.Visit(Anchor.New(this, parent));
                            else visitor.Visit(Anchor.New<{recordSymbol.languageAgnosticType()?.ToString() ?? $"Syntax<{langName}>"}>(this, parent));
                        }}"))
                !;
        }
    }
}
