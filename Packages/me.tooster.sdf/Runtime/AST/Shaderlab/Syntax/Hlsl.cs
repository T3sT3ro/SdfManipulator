#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    [SyntaxNode] public partial record HlslInclude : SubShaderOrPassStatement {
        public HlslIncludeKeyword                 hlslIncludeKeyword { get; init; } = new();
        public InjectedLanguage<shaderlab, hlsl>? hlsl               { get; init; }
        public EndHlslKeyword                     endHlslKeyword     { get; init; } = new();
    }

    [SyntaxNode] public partial record HlslProgram : PassStatement {
        public HlslProgramKeyword                 hlslProgramKeyword { get; init; } = new();
        public InjectedLanguage<shaderlab, hlsl>? hlsl               { get; init; }
        public EndHlslKeyword                     endHlslKeyword     { get; init; } = new();
    }
}
