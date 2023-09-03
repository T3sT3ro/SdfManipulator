#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    public record HlslInclude : SubShaderOrPassStatement {
        public HlslIncludeKeyword                      _hlslIncludeKeyword { get; init; } = new();
        public InjectedLanguage<Shaderlab, Hlsl.Hlsl>? _hlsl               { get; init; } = null;
        public EndHlslKeyword                          _endHlslKeyword     { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>?[]
            { hlslIncludeKeyword, hlsl, endHlslKeyword }.FilterNotNull().ToList();
    }

    public record HlslProgram : PassStatement {
        public HlslProgramKeyword                      _hlslProgramKeyword { get; init; } = new();
        public InjectedLanguage<Shaderlab, Hlsl.Hlsl>? _hlsl               { get; init; }
        public EndHlslKeyword                          _endHlslKeyword     { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>?[]
            { hlslProgramKeyword, hlsl, endHlslKeyword }.FilterNotNull().ToList();
    }
}
