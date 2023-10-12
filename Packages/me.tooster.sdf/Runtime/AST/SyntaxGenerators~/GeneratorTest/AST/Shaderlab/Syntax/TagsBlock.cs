using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    /// <a href="https://docs.unity3d.com/Manual/SL-SubShaderTags.html">Tags</a>
    [AstSyntax] public partial record TagsBlock : SubShaderOrPassStatement {
        public TagsKeyword                tagsKeyword { get; init; } = new();
        public OpenBraceToken             openBraceToken { get; init; } = new();
        public SyntaxList<Shaderlab, Tag> tags { get; init; } = new();
        public CloseBraceToken            closeBraceToken { get; init; } = new();
    }
}
