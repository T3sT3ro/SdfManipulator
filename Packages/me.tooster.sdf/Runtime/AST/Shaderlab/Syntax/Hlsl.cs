#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    public record HlslInclude : SubShaderOrPassStatement {
        public HlslIncludeKeyword                      hlslIncludeKeyword { get; init; } = new();
        public InjectedLanguage<Shaderlab, Hlsl.Hlsl>? hlsl               { get; init; } = null;
        public EndHlslKeyword                          endHlslKeyword     { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>?[]
            { hlslIncludeKeyword, hlsl, endHlslKeyword }.FilterNotNull().ToList();
    }

    public record HlslProgram : PassStatement {
        public HlslProgramKeyword                      hlslProgramKeyword { get; init; } = new();
        public InjectedLanguage<Shaderlab, Hlsl.Hlsl>? hlsl               { get; init; }
        public EndHlslKeyword                          endHlslKeyword     { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>?[]
            { hlslProgramKeyword, hlsl, endHlslKeyword }.FilterNotNull().ToList();
    }
}
