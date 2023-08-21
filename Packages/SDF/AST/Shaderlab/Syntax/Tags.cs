using System.Collections.Generic;
using System.Linq;

namespace AST.Shaderlab.Syntax {
    /// <a href="https://docs.unity3d.com/Manual/SL-SubShaderTags.html">Tags</a>
    public record Tags : SubShaderOrPassStatement {
        public TagsKeyword        tagsKeyword     { get; set; } = new();
        public OpenBraceToken     openBraceToken  { get; set; } = new();
        public IReadOnlyList<Tag> tags            { get; set; }
        public CloseBraceToken    closeBraceToken { get; set; } = new();

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
                { tagsKeyword, openBraceToken }
            .Concat(tags)
            .Append(closeBraceToken)
            .ToList();
    }
}
