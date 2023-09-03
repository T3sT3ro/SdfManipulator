#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    [Syntax] public partial record HlslInclude : SubShaderOrPassStatement {
        [Init] private readonly HlslIncludeKeyword                      _hlslIncludeKeyword;
        private readonly        InjectedLanguage<Shaderlab, Hlsl.Hlsl>? _hlsl;
        [Init] private readonly EndHlslKeyword                          _endHlslKeyword;
    }

    [Syntax] public partial record HlslProgram : PassStatement {
        [Init] private readonly HlslProgramKeyword                      _hlslProgramKeyword;
        private readonly        InjectedLanguage<Shaderlab, Hlsl.Hlsl>? _hlsl;
        [Init] private readonly EndHlslKeyword                          _endHlslKeyword;
    }
}
