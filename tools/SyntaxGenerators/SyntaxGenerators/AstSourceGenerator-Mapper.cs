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
        void GenerateMapper(SymbolSet ss) {
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
        ClassDeclarationSyntax generateMapperClass(SymbolSet ss) {
            var visitor = (ClassDeclarationSyntax)ParseMemberDeclaration(
                $"public partial class Mapper : AST.Mapper<{ss.LangName.ToLower()}>, Visitor<Tree<{ss.LangName.ToLower()}>.Node> {{"
              + $"public Mapper() : base() {{}}"
              + $"}}"
            )!;


            var syntaxVisitMethods = ss.Syntaxes.Select(record => {
                var paramTypeName = record.getFullyQualifiedTypeNameWithGenerics(ss.LangName, out var tp);
                var parameterType = IdentifierName($"Anchor<{paramTypeName}>");
                TypeSyntax returnType = IdentifierName(Identifier($"Tree<{ss.LangName.ToLower()}>.Node?"));

                // visit children in syntax order, the same order as ChildNodesAndTokens
                var ownProperties = Utils.OwnProperties(record).Where(Utils.isPropertyCompatible);
                var inheritedProperties = Utils.InheritedProperties(record).Where(Utils.isPropertyCompatible);
                var inheritedAndOwnProperties = OrderLikeChildNodesAndTokens(record, inheritedProperties.Concat(ownProperties).ToList());

                var methodDeclaration = (MethodDeclarationSyntax)ParseMemberDeclaration(
                    $"public virtual Tree<{ss.LangName.ToLower()}>.Node? Visit(Anchor<{record}> a) {{}}"
                )!;

                if (tp.Count > 0) {
                    var constraints = (record as INamedTypeSymbol)
                        ?.TypeParameters.Select(t => TypeParameterConstraintClause(t.Name)
                            .AddConstraints(t.ConstraintTypes.Select(c =>
                                TypeConstraint(IdentifierName(c.ToString()))).ToArray()));

                    methodDeclaration = methodDeclaration
                        .WithTypeParameterList(TypeParameterList(SeparatedList(tp.Select(TypeParameter))))
                        .AddConstraintClauses(constraints.ToArray());
                }

                methodDeclaration = methodDeclaration.WithBody(record.IsAbstract
                    // dispatch to the concrete type; Accept takes the anchor of the parent, not of the node itself
                    ? Block(ParseStatement("return a.Node.Accept(this, a.Parent);"))
                    : Block(mapperVisitOverride(inheritedAndOwnProperties, ss.LangName)));

                return methodDeclaration;
            }).Where(x => x is not null);

            return visitor.AddMembers(syntaxVisitMethods.ToArray()).WithAttributeLists(generatedAttributeSyntax);
        }

        /// <summary>
        ///     Base members come first, which is syntax order unless the record declares its own ChildNodesAndTokens: a
        ///     preprocessor directive starts with the <c>#</c> of its base, but ends with the end of directive of its base.
        ///     In that case the order of the properties in the handwritten ChildNodesAndTokens wins.
        /// </summary>
        static List<IPropertySymbol> OrderLikeChildNodesAndTokens(ITypeSymbol record, List<IPropertySymbol> properties) {
            var names = record.GetMembers("ChildNodesAndTokens")
                .SelectMany(m => m.DeclaringSyntaxReferences)
                .SelectMany(r => r.GetSyntax().DescendantNodes().OfType<IdentifierNameSyntax>())
                .Select(id => id.Identifier.ValueText)
                .ToList();
            if (names.Count == 0)
                return properties;

            int position(IPropertySymbol p) {
                var index = names.IndexOf(p.Name);
                return index < 0 ? int.MaxValue : index;
            }

            return properties.Select((p, i) => (p, i)).OrderBy(t => position(t.p)).ThenBy(t => t.i).Select(t => t.p).ToList();
        }

        static IEnumerable<StatementSyntax> mapperVisitOverride(List<IPropertySymbol> props, string langName) {
            foreach (var p in props) {
                var isToken = p.Type.isAstToken();
                yield return (LocalDeclarationStatementSyntax)ParseStatement(
                    $"var {p.Name} = a.Node.{p.Name} is null ? null : Visit(Anchor.New{(isToken ? $"<Token<{langName.ToLower()}>>" : "")}(a.Node.{p.Name}, a)) as {p.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated)};");
            }

            yield return ParseStatement(
                $"if({string.Join(" && ", props.Select(p => $"ReferenceEquals({p.Name}, a.Node.{p.Name})"))}) return a.Node;");

            yield return ParseStatement(
                $"return a.Node with {{ {string.Join(", ", props.Select(p => $"{p.Name} = {p.Name}!"))} }};");
        }
    }
}
