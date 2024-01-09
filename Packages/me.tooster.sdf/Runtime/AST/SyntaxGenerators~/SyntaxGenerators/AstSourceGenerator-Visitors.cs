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
        private void GenerateVisitor(SymbolSet ss) {
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
                            generateVisitorInterface(ss, false),
                            generateVisitorInterface(ss, true)
                        )
                ).WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)));

            context.AddSource($"{ss.LangName}.Visitors.g.cs",
                SourceText.From(compilationUnit.NormalizeWhitespace().SyntaxTree.ToString(), Encoding.UTF8)
            );
        }

        // generates methods like Visit(ForSyntax<hlsl> node)
        private InterfaceDeclarationSyntax generateVisitorInterface(SymbolSet ss, bool withReturnTypes) {
            var visitor = InterfaceDeclaration(Identifier($"Visitor{(withReturnTypes ? "<R>" : "")}"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(
                    SimpleBaseType(
                        IdentifierName($"AST.Visitor<{ss.LangName.ToLower()}{(withReturnTypes ? ", R" : "")}>")))))
                .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));

            var syntaxVisitMethods = ss.Syntaxes.Select(record => {
                // var recordName = Utils.getTypeNameWithGenericArguments(record);
                var isTypeGeneric = record is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol;
                var parameterName = "node";
                var paramTypeName = record.getFullyQualifiedTypeNameWithGenerics(ss.LangName, out var tp);
                var parameterType = IdentifierName($"Anchor<{paramTypeName}>");
                TypeSyntax returnType = withReturnTypes
                    ? NullableType(IdentifierName("R"))
                    : PredefinedType(Token(SyntaxKind.VoidKeyword));

                // empty block or returning default
                var body = withReturnTypes
                    ? Block(ReturnStatement(DefaultExpression(IdentifierName("R"))))
                    : Block();

                var constraints = (record as INamedTypeSymbol)
                    ?.TypeParameters.Select(t => TypeParameterConstraintClause(t.Name)
                        .AddConstraints(t.ConstraintTypes.Select(c => TypeConstraint(IdentifierName(c.ToString()))).ToArray()));
                
                var method = MethodDeclaration(returnType, Identifier("Visit"))
                    .WithTypeParameterList(tp.Count > 0
                        ? TypeParameterList(SeparatedList(tp.Select(TypeParameter)))
                        : null)
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                    .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier(parameterName))
                        .WithType(parameterType))))
                    .WithBody(body);
                
                if(constraints != null)
                    method = method.WithConstraintClauses(List(constraints));
                return method;
            });

            return visitor.AddMembers(syntaxVisitMethods.ToArray());
        }
    }
}
