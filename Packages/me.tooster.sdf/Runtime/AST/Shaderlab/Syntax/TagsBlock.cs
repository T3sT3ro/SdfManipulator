using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    /// <a href="https://docs.unity3d.com/Manual/SL-SubShaderTags.html">Tags</a>
    [SyntaxNode] public partial record TagsBlock : SubShaderOrPassStatement {
        public TagsKeyword                tagsKeyword     { get; init; } = new();
        public OpenBraceToken             openBraceToken  { get; init; } = new();
        public SyntaxList<shaderlab, Tag> tags            { get; init; } = new();
        public CloseBraceToken            closeBraceToken { get; init; } = new();
    }
}
