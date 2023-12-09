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
        private void GenerateMapper(SymbolSet ss) {
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
                        .AddMembers(generateMapperClass(ss))
                ).WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)));

            context.AddSource($"{ss.LangName}.Mapper.g.cs",
                SourceText.From(compilationUnit.NormalizeWhitespace().SyntaxTree.ToString(), Encoding.UTF8)
            );
        }

        // generates methods like Visit(ForSyntax<hlsl> node)
        private ClassDeclarationSyntax generateMapperClass(SymbolSet ss) {
            var visitor = ClassDeclaration(Identifier($"Mapper<TState>"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword)))
                .AddBaseListTypes(
                    SimpleBaseType(IdentifierName($"AST.Mapper<{ss.LangName.ToLower()}, TState>")),
                    SimpleBaseType(IdentifierName($"Visitor<Tree<{ss.LangName.ToLower()}>.Node>")))
                .AddConstraintClauses(TypeParameterConstraintClause("TState")
                    .AddConstraints(TypeConstraint(IdentifierName("MapperState")), ConstructorConstraint()))
                .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken))
                .AddMembers(ConstructorDeclaration(Identifier("Mapper"))
                    .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)))
                    .AddParameterListParameters(Parameter(Identifier("state")).WithType(IdentifierName("TState")))
                    .WithInitializer(ConstructorInitializer(SyntaxKind.BaseConstructorInitializer,
                        ArgumentList(SingletonSeparatedList(Argument(IdentifierName("state"))))))
                    .WithBody(Block()));
            
            var syntaxVisitMethods = ss.Syntaxes.Select(record => {
                var isTypeGeneric = record is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol;
                var parameterName = "a";
                var paramTypeName = Utils.getFullyQualifiedTypeNameWithGenerics(record, ss.LangName, out var tp);
                var parameterType = IdentifierName($"Anchor<{paramTypeName}>");
                TypeSyntax returnType = IdentifierName(Identifier($"Tree<{ss.LangName.ToLower()}>.Node?"));

                var constraints = (record as INamedTypeSymbol)
                    ?.TypeParameters.Select(t => TypeParameterConstraintClause(t.Name)
                        .AddConstraints(t.ConstraintTypes.Select(c => TypeConstraint(IdentifierName(c.ToString())))
                            .ToArray()));

                var ownProperties = Utils.getOwnProperties(record).ToList();
                var inheritedProperties = Utils.getInheritedProperties(record).ToList();
                var inheritedAndOwnProperties = ownProperties.Concat(inheritedProperties).ToList();
                
                var method = MethodDeclaration(returnType, Identifier($"Visitor<Tree<{ss.LangName.ToLower()}>.Node>.Visit"))
                    .WithTypeParameterList(tp.Count > 0
                        ? TypeParameterList(SeparatedList(tp.Select(TypeParameter)))
                        : null)
                    .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier(parameterName))
                        .WithType(parameterType))))
                    .WithBody(Block(mapperFunctionStatements(record, ownProperties.Concat(inheritedProperties))));

                return method;
            }).Where(x => x is not null);

            return visitor.AddMembers(syntaxVisitMethods.ToArray());
        }
    }
}
