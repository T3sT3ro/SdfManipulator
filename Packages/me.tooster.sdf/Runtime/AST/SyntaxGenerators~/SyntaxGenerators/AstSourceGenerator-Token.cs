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
        public void GenerateTokenPartials(IEnumerable<ITypeSymbol> tokens) {
            var langGroups = tokens.GroupBy(Utils.getLangName);

            foreach (var langGroup in langGroups) {
                var langName = langGroup.Key;
                var compilationUnit = CompilationUnit()
                    .AddUsings(
                        UsingDirective(IdentifierName("System.Collections.Generic")),
                        UsingDirective(IdentifierName($"{ROOT_NAMESPACE}.Syntax")),
                        UsingDirective(IdentifierName($"{ROOT_NAMESPACE}.{langName}"))
                    )
                    .AddMembers(
                        NamespaceDeclaration(IdentifierName(langGroup.First().ContainingNamespace.ToDisplayString()))
                            .AddMembers(langGroup.Select(record => generateTokenPartial(langName, record)).ToArray())
                    );

                context.AddSource($"{langName}.Tokens.g.cs",
                    SourceText.From(compilationUnit.NormalizeWhitespace().SyntaxTree.ToString(), Encoding.UTF8)
                );
            }
        }

        private MemberDeclarationSyntax generateTokenPartial(string langName, ITypeSymbol token) {
            // if it has base list already, don't add it
            
            var tokenRecordDeclaration = RecordDeclaration(Token(SyntaxKind.RecordKeyword),
                    Identifier(Utils.getTypeNameWithGenericArguments(token)))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword)))
                .AddMembers(PropertyDeclaration(
                        PredefinedType(Token(SyntaxKind.StringKeyword)),
                        Identifier("FullText"))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                    .WithExpressionBody(ArrowExpressionClause(LiteralExpression(SyntaxKind.StringLiteralExpression,
                        Literal(token.GetAttributes()
                            .Single(attr => attr.AttributeClass!.Name == TOKEN_ATTRIBUTE_NAME)
                            .ConstructorArguments[0].Value as string ?? string.Empty))))
                    .WithSemicolonToken(
                        Token(SyntaxKind.SemicolonToken))
                )
                .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));
            
            return token.BaseType?.IsAbstract == true
                ? tokenRecordDeclaration
                : tokenRecordDeclaration.AddBaseListTypes(SimpleBaseType(IdentifierName($"Token<{langName.ToLower()}>")));
        }
    }
}
