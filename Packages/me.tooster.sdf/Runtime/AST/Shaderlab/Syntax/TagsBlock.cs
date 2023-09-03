using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    /// <a href="https://docs.unity3d.com/Manual/SL-SubShaderTags.html">Tags</a>
    public record TagsBlock : SubShaderOrPassStatement {
        public TagsKeyword                _tagsKeyword     { get; init; } = new();
        public OpenBraceToken             _openBraceToken  { get; init; } = new();
        public SyntaxList<Shaderlab, Tag> _tags            { get; init; }
        public CloseBraceToken            _closeBraceToken { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { tagsKeyword, openBraceToken, tags, closeBraceToken };
    }
}
