using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    /// <a href="https://docs.unity3d.com/Manual/SL-SubShaderTags.html">Tags</a>
    [Syntax] public partial record  TagsBlock : SubShaderOrPassStatement {
        [Init] private readonly TagsKeyword                _tagsKeyword     ;
        [Init] private readonly OpenBraceToken             _openBraceToken  ;
        [Init] private readonly SyntaxList<Shaderlab, Tag> _tags            ;
        [Init] private readonly CloseBraceToken            _closeBraceToken ;
    }
}
