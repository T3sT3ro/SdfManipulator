using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ITypeSymbol = Microsoft.CodeAnalysis.ITypeSymbol;

namespace SDF.SourceGen {
    // This generator collects all partial classes with [Syntax] attribute
    // Each partial class will have a property (with trailing underscore stripped) with getter and init setter
    // where getter returns the field and setter assigns the field and uses the with { Parent = this }; syntax
    // This is just a broad phase to sieve required types, it must be very minimal and performant.
    // Runs once for many nodes
    public class AstReceiver : ISyntaxContextReceiver {
        public Dictionary<ITypeSymbol, SyntaxList<UsingDirectiveSyntax>> Records { get; } = new();


        public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
            if (context.Node is not RecordDeclarationSyntax recordSyntax
             || !isRecordCompatible(context, recordSyntax))
                return;

            var symbol = (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, recordSyntax) as ITypeSymbol)!;
            var exists = Records.TryGetValue(symbol, out var usings);
            if(!exists)
                usings = new SyntaxList<UsingDirectiveSyntax>();

            ;
            Records[symbol] = usings.AddRange(recordSyntax.Ancestors().OfType<CompilationUnitSyntax>().Single().Usings);
        }


        // returns fields from inherited records which are of SyntaxOrToken subtype

        private bool isRecordCompatible(GeneratorSyntaxContext ctx, RecordDeclarationSyntax recordSyntax) {
            // must be partial
            if (!recordSyntax.Modifiers.Any(tok => tok.IsKind(SyntaxKind.PartialKeyword)))
                return false;

            // must be derived from Syntax<T>
            if (!isDerivedFromSyntax(ModelExtensions.GetDeclaredSymbol(ctx.SemanticModel, recordSyntax) as ITypeSymbol))
                return false;

            // require marker attribute
            var markerAttribute = ctx.SemanticModel.Compilation.GetTypeByMetadataName($"{AstSourceGenerator.SYNTAX_ATTRIBUTE_NAME}Attribute");
            var astAttribute = ModelExtensions.GetDeclaredSymbol(ctx.SemanticModel, recordSyntax)
                ?.GetAttributes()
                .Any(ad => ad.AttributeClass?.Equals(markerAttribute, SymbolEqualityComparer.Default) ?? false);

            return astAttribute ?? false;
        }

        private bool isDerivedFromSyntax(ITypeSymbol? typeSymbol) {
            while (typeSymbol != null) {
                if (typeSymbol.BaseType?.ConstructedFrom.ToString() == "me.tooster.sdf.AST.Syntax.Syntax<Lang>")
                    return true;

                typeSymbol = typeSymbol.BaseType;
            }

            return false;
        }
    }
}
