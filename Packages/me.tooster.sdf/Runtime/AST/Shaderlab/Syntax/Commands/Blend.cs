#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    /// <a href="https://docs.unity3d.com/Manual/SL-Blend.html">Blend</a>
    public record Blend : Command {
        public BlendKeyword     _blendKeyword   { get; init; } = new();
        public CommandArgument? _renderTarget   { get; init; }
        public BlendArguments   _blendArguments { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>?[]
            { blendKeyword, renderTarget, blendArguments }.FilterNotNull().ToList();


        public abstract record BlendArguments : CommandArgument;

        public record StateArgument : BlendArguments {
            public CommandArgument _stateArg { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new[]
                { stateArg };
        }

        public record SrcDstArguments : BlendArguments {
            public CommandArgument _srcFactorArg { get; init; }
            public CommandArgument _dstFactorArg { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new[]
                { srcFactorArg, dstFactorArg };
        }

        public record RgbaFactorArguments : BlendArguments {
            public CommandArgument _srcRgbFactor   { get; init; }
            public CommandArgument _srcAlphaFactor { get; init; }
            public CommaToken      _commaToken     { get; init; } = new();
            public CommandArgument _dstRgbFactor   { get; init; }
            public CommandArgument _dstAlphaFactor { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens =>
                new SyntaxOrToken<Shaderlab>[]
                    { srcRgbFactor, srcAlphaFactor, commaToken, dstRgbFactor, dstAlphaFactor };
        }
    }
}
