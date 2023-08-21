using System.Collections.Generic;
using System.Text;

namespace AST.Syntax {
    // todo: won't abstract class just work?
    public interface ISyntaxToken<TNode, TToken, TTrivia, TBase>
        : ISyntaxNodeOrToken<TNode, TBase>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TToken : ISyntaxToken<TNode, TToken, TTrivia, TBase>, TBase
        where TTrivia : SyntaxTrivia<TToken, TNode, TTrivia, TBase>
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {
        IReadOnlyList<TTrivia> LeadingTrivia  { get; }
        IReadOnlyList<TTrivia> TrailingTrivia { get; }

        /// When used for example in literal as '12.0f' returns '12.0f'.
        /// In roslyn there are additional 'valueText', for example when text is '@interface', the valueText is 'interface'.
        /// There is also a Value property which would return (object) 12 for '12.0f'. For now `Text` suffices.
        public string Text { get; }

        void ISyntaxNodeOrToken<TNode, TBase>.WriteTo(StringBuilder sb) {
            foreach (var leading in LeadingTrivia) sb.Append(leading);
            sb.Append(Text);
            foreach (var trailing in TrailingTrivia) sb.Append(trailing);
        }
    }
}
