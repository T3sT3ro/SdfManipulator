using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace me.tooster.sdf.AST.Generators {
    public partial class AstSourceGenerator {
        private void GenerateVisitorPartials(SymbolSet ss) {
            var compilationUnit = CompilationUnit()
                .AddUsings(
                    UsingDirective(IdentifierName("System")),
                    UsingDirective(IdentifierName("System.Collections.Generic")),
                    UsingDirective(IdentifierName($"{ROOT_NAMESPACE}.Syntax")),
                    UsingDirective(IdentifierName($"{ROOT_NAMESPACE}.{ss.LangName}")),
                    UsingDirective(IdentifierName($"{ROOT_NAMESPACE}.{ss.LangName}.Syntax"))
                )
                .AddMembers(
                    NamespaceDeclaration(IdentifierName($"{ROOT_NAMESPACE}.{ss.LangName}"))
                        .AddMembers(
                            generateVisitor(ss, false),
                            generateVisitor(ss, true)
                        )
                );

            context.AddSource($"{ss.LangName}.Visitors.g.cs",
                SourceText.From(compilationUnit.NormalizeWhitespace().SyntaxTree.ToString(), Encoding.UTF8)
            );
        }
        
        // generates methods like Visit(ForSyntax<hlsl> node)
        private ClassDeclarationSyntax generateVisitor(SymbolSet ss, bool withReturnTypes) {
            var visitor = ClassDeclaration(Identifier($"Visitor{(withReturnTypes ? "<TRet>" : "")}"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword)))
                .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(
                    SimpleBaseType(IdentifierName($"AST.Visitor<{ss.LangName.ToLower()}{(withReturnTypes ? ", TRet" : "")}>")))))
                .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));
            
            var syntaxVisitMethods = ss.Syntaxes.Select(record => {
                var recordName = Utils.getTypeNameWithGenericArguments(record);
                var isTypeGeneric = record is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol;
                var methodName = $"Visit{(withReturnTypes ? "<TRet>" : "")}";
                var parameterName = "node";
                var parameterType = IdentifierName(Utils.getFullyQualifiedTypeNameWithGenerics(record, ss.LangName, out var tp));
                TypeSyntax returnType = withReturnTypes ? IdentifierName("TRet") : PredefinedType(Token(SyntaxKind.VoidKeyword));
                // empty block or returning default
                var body = withReturnTypes
                    ? Block(ReturnStatement(DefaultExpression(IdentifierName("TRet"))))
                    : Block();
                return MethodDeclaration(returnType, Identifier(methodName))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                    .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier(parameterName))
                        .WithType(parameterType))))
                    .WithBody(body);
            });

            return visitor.AddMembers(syntaxVisitMethods.ToArray());
        }
    }
}
