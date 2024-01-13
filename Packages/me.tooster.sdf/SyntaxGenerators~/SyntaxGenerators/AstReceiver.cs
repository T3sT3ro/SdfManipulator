#nullable enable
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ITypeSymbol = Microsoft.CodeAnalysis.ITypeSymbol;

// This generator collects all partial classes with [Syntax] attribute
// Each partial class will have a property (with trailing underscore stripped) with getter and init setter
// where getter returns the field and setter assigns the field and uses the with { Parent = this }; syntax
// This is just a broad phase to sieve required types, it must be very minimal and performant.
// Runs once for many nodes
namespace me.tooster.sdf.AST.Generators {
    public class SymbolSet {
        public SymbolSet(string langName) => LangName = langName;
        public string                                              LangName { get; }
        public HashSet<ITypeSymbol>                                Syntaxes { get; } = new();
        public HashSet<ITypeSymbol>                                Tokens   { get; } = new();
        public HashSet<ITypeSymbol>                                Trivia   { get; } = new();
        public Dictionary<ITypeSymbol, List<UsingDirectiveSyntax>> Includes { get; } = new();
    }

    public class AstReceiver : ISyntaxContextReceiver {
        public Dictionary<string, SymbolSet> LanguageSymbols { get; } = new();

        public List<Diagnostic> Diagnostics { get; } = new();

        public ISymbol? syntaxAttribute;
        public ISymbol? tokenAttribute;
        public ISymbol? triviaAttribute;

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
            if (context.Node is not RecordDeclarationSyntax recordSyntax)
                return;

            syntaxAttribute ??= context.SemanticModel.Compilation.GetTypeByMetadataName(
                $"{AstSourceGenerator.ROOT_NAMESPACE}.{AstSourceGenerator.SYNTAX_ATTRIBUTE_NAME}");
            tokenAttribute ??= context.SemanticModel.Compilation.GetTypeByMetadataName(
                $"{AstSourceGenerator.ROOT_NAMESPACE}.{AstSourceGenerator.TOKEN_ATTRIBUTE_NAME}");
            triviaAttribute ??= context.SemanticModel.Compilation.GetTypeByMetadataName(
                $"{AstSourceGenerator.ROOT_NAMESPACE}.{AstSourceGenerator.TRIVIA_ATTRIBUTE_NAME}");

            var symbol = (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, recordSyntax) as ITypeSymbol)!;


            var isSyntax = symbol.isAnnotated(syntaxAttribute!);
            var isToken = symbol.isAnnotated(tokenAttribute!);
            var isTrivia = symbol.isAnnotated(triviaAttribute!);
            if (!isToken && !isSyntax && !isTrivia) return;

            var lang = symbol.getLangName();
            LanguageSymbols.TryGetValue(lang, out var ss);
            ss ??= LanguageSymbols[lang] = new SymbolSet(lang);

            if (isSyntax) {
                if (!assertPartial(recordSyntax, symbol)) return;
                if (!assertAccess(symbol, recordSyntax, Accessibility.Public)) return;
                if (!assertSyntaxDerived(symbol, recordSyntax)) return;

                ss.Syntaxes.Add(symbol);
                // record all #include <something> rules declared in the syntax clas
                ss.Includes[symbol] = recordSyntax.Ancestors().OfType<CompilationUnitSyntax>()
                    .SelectMany(cu => cu.Usings).ToList();
                
            } else if (isToken) {
                if (!assertPartial(recordSyntax, symbol)) return;

                ss.Tokens.Add(symbol);
            } else if (isTrivia) {
                ss.Trivia.Add(symbol);
            }
        }

        private bool assertAccess(ITypeSymbol symbol, RecordDeclarationSyntax recordSyntax,
            Accessibility accessibility) {
            if (symbol.DeclaredAccessibility == accessibility) return true;

            Diagnostics.Add(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "AST_REQUIRE_PUBLIC",
                    "Record must be public",
                    $"Record {symbol.Name} must be public",
                    "AST-Syntax",
                    DiagnosticSeverity.Error,
                    true),
                recordSyntax.GetLocation()));
            return false;
        }

        /// asserts, with diagnostic when needed, that a type is derived from Syntax or is annotated with [Syntax] for generation 
        private bool assertSyntaxDerived(ITypeSymbol symbol, RecordDeclarationSyntax recordSyntax) {
            if (symbol.BaseTypes().Any(type => type is INamedTypeSymbol { ConstructedFrom: { Name: "Syntax" } }
                 || type.isAnnotated(syntaxAttribute!))) {
                return true;
            }

            Diagnostics.Add(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "AST_REQUIRE_SYNTAX",
                    $"Record must be derived from Syntax",
                    $"Record {symbol.Name} must be derived from Syntax",
                    "AST-Syntax",
                    DiagnosticSeverity.Error,
                    true),
                recordSyntax.GetLocation()));
            return false;
        }

        private bool assertPartial(RecordDeclarationSyntax recordSyntax, ITypeSymbol symbol) {
            if (recordSyntax.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PartialKeyword))) return true;

            Diagnostics.Add(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "AST_REQUIRE_PARTIAL",
                    "Record must be partial",
                    $"Record {symbol.Name} must be partial",
                    "AST-Token",
                    DiagnosticSeverity.Error,
                    true),
                recordSyntax.GetLocation()));

            return false;
        }
    }
}
