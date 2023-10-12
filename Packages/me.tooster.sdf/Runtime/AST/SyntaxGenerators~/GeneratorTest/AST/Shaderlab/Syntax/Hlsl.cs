#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    [AstSyntax] public partial record HlslInclude : SubShaderOrPassStatement {
        public HlslIncludeKeyword                      hlslIncludeKeyword { get; init; } = new();
        public        InjectedLanguage<Shaderlab, Hlsl.Hlsl>? hlsl { get; init; }
        public EndHlslKeyword                          endHlslKeyword { get; init; } = new();
    }

    [AstSyntax] public partial record HlslProgram : PassStatement {
        public HlslProgramKeyword                      hlslProgramKeyword { get; init; } = new();
        public        InjectedLanguage<Shaderlab, Hlsl.Hlsl>? hlsl { get; init; }
        public EndHlslKeyword                          endHlslKeyword { get; init; } = new();
    }
}
