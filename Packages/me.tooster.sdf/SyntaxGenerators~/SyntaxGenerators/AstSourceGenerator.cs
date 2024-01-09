using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

// from: https://medium.com/@EnescanBektas/using-source-generators-in-the-unity-game-engine-140ff0cd0dc

namespace me.tooster.sdf.AST.Generators {
    [Generator]
    public partial class AstSourceGenerator : ISourceGenerator {
        internal const string SYNTAX_ATTRIBUTE_NAME = "SyntaxNodeAttribute";
        internal const string TOKEN_ATTRIBUTE_NAME  = "TokenNodeAttribute";
        internal const string TRIVIA_ATTRIBUTE_NAME = "TriviaNodeAttribute";

        public const           string GROUP          = "me.tooster.sdf";
        public const           string AST_NAMESPACE  = "AST";
        public static readonly string ROOT_NAMESPACE = $"{GROUP}.{AST_NAMESPACE}";

        internal static readonly string ATTRIBUTES =
            @$"#nullable enable
using System;
    /*
     * Below markers are used in by the source generator to generate Red AST nodes (lazy, with parents)  
     */

namespace {ROOT_NAMESPACE} {{

    /// Marks Syntax for generation
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    internal class {SYNTAX_ATTRIBUTE_NAME} : Attribute {{}}

    /// Marks Token for generation
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    internal class {TOKEN_ATTRIBUTE_NAME} : Attribute {{
        public string Text;
        public {TOKEN_ATTRIBUTE_NAME}(string text = """") {{ this.Text = text; }}
    }}

    /// Marks Trivia for generation
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    internal class {TRIVIA_ATTRIBUTE_NAME} : Attribute {{}}
}}
";

        // private Compilation               compilation;
        private GeneratorExecutionContext context;

        public void Initialize(GeneratorInitializationContext initializationContext) {
            initializationContext.RegisterForPostInitialization(i => i.AddSource("SyntaxAttributes.g.cs", ATTRIBUTES));
            initializationContext.RegisterForSyntaxNotifications(() => new AstReceiver());
        }

        public void Execute(GeneratorExecutionContext executionContext) {
            if (!(executionContext.SyntaxContextReceiver is AstReceiver receiver))
                return;
        
            context = executionContext;

            receiver.Diagnostics.ForEach(context.ReportDiagnostic);

            foreach (var kv in receiver.LanguageSymbols) {
                var langName = kv.Key;
                var ss = kv.Value;
                
                GenerateLanguageMarkerInterface(langName);
                
                GenerateTokenPartials(ss.Tokens, langName);
                
                foreach (var recordSymbol in ss.Syntaxes)
                    GenerateSyntaxPartials(recordSymbol, ss);
                
                GenerateVisitor(ss);
                GenerateMapper(ss);
            }
            

        }
        // ------------------- GENERATION -------------------

        private void GenerateLanguageMarkerInterface(string langName) {
            var langMarker = @$"
namespace {ROOT_NAMESPACE} {{
    /// marker interface for {langName} language
    public interface {langName.ToLower()} {{}}
}}
";

            context.AddSource($"Languages.{langName}.g.cs", SourceText.From(langMarker, Encoding.UTF8)
            );
        }


        private static NamespaceDeclarationSyntax GenerateSyntaxNamespace(ITypeSymbol record) {
            var targetNamespace = record.ContainingNamespace.ToString();

            return NamespaceDeclaration(IdentifierName(targetNamespace))
                .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));
        }
    }
}
