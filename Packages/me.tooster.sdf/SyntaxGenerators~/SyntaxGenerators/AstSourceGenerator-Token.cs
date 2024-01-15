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
        public void GenerateTokenPartials(IEnumerable<ITypeSymbol> tokens, string langName) {
            var compilationUnit = CompilationUnit()
                .AddUsings(
                    UsingDirective(IdentifierName("System.Collections.Generic")),
                    UsingDirective(IdentifierName($"{ROOT_NAMESPACE}.Syntax")),
                    UsingDirective(IdentifierName($"{ROOT_NAMESPACE}.{langName}"))
                )
                .AddMembers(
                    /*NamespaceDeclaration(IdentifierName($"{ROOT_NAMESPACE}.{langName}.Syntax"))
                        .AddMembers(tokens.Select(record => generateTokenPartial(record, langName)).ToArray())*/
                    tokens.GroupBy(token => token.ContainingNamespace.ToString())
                        .Select(tokenGroup => NamespaceDeclaration(IdentifierName(tokenGroup.Key))
                            .AddMembers(tokenGroup.Select(record => generateTokenPartial(record, langName)).ToArray()))
                        .ToArray()
                );

            context.AddSource($"{langName}.Tokens.g.cs",
                SourceText.From(compilationUnit.NormalizeWhitespace().SyntaxTree.ToString(), Encoding.UTF8)
            );
        }

        private MemberDeclarationSyntax generateTokenPartial(ITypeSymbol tokenSymbol, string langName) {
            // if it has base list already, don't add it

            var tokenRecordDeclaration = RecordDeclaration(Token(SyntaxKind.RecordKeyword),
                    Identifier(tokenSymbol.getTypeNameWithGenericArguments()))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword)))
                .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));

            if (tokenSymbol.BaseType?.IsAbstract == false)
                tokenRecordDeclaration =
                    tokenRecordDeclaration.AddBaseListTypes(
                        SimpleBaseType(IdentifierName($"Token<{langName.ToLower()}>")));

            if (!tokenSymbol.IsAbstract && tokenSymbol.OwnProperties("Text")
                    .Concat(tokenSymbol.InheritedProperties("Text"))
                    .All(m => m.IsAbstract))
                tokenRecordDeclaration = tokenRecordDeclaration.AddMembers(PropertyDeclaration(
                            PredefinedType(Token(SyntaxKind.StringKeyword)),
                            Identifier("Text"))
                        .WithModifiers(
                            TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                        .WithExpressionBody(ArrowExpressionClause(LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            Literal(tokenSymbol.GetAttributes()
                                .Single(attr => attr.AttributeClass!.Name == TOKEN_ATTRIBUTE_NAME)
                                .ConstructorArguments[0].Value as string ?? string.Empty))))
                        .WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken)) /*,
                        Utils.GenerateVisitorAcceptor(langName),
                        Utils.GenerateReturningVisitorAcceptor(langName)*/
                );

            if (!tokenSymbol.GetMembers("ToString").Any(m => !m.IsImplicitlyDeclared) /* && !recordSymbol.IsAbstract*/
               ) {
                tokenRecordDeclaration = tokenRecordDeclaration.AddMembers(ParseMemberDeclaration(
                    "public override string ToString() => base.ToString();")!);
            }

            return tokenRecordDeclaration;
        }
    }
}
