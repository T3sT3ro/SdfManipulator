using System.Collections.Generic;

namespace AST.Syntax {
    public interface ISyntaxToken<TNode, TTrivia, TBase>
        : ISyntaxNodeOrToken<TNode, TBase>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {
        
        IReadOnlyList<TTrivia> LeadingTrivia  { get; }
        IReadOnlyList<TTrivia> TrailingTrivia { get; }

        /// When used for example in literal as '12.0f' returns '12.0f'.
        /// In roslyn there are additional 'valueText', for example when text is '@interface', the valueText is 'interface'.
        /// There is also a Value property which would return (object) 12 for '12.0f'. For now `Text` suffices.
        public string Text { get; }
    }
}
