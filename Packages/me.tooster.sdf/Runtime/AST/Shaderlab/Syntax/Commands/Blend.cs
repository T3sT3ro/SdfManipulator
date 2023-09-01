#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    /// <a href="https://docs.unity3d.com/Manual/SL-Blend.html">Blend</a>
    public record Blend : Command {
        public BlendKeyword     blendKeyword   { get; init; } = new();
        public CommandArgument? renderTarget   { get; init; }
        public BlendArguments   blendArguments { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>?[]
            { blendKeyword, renderTarget, blendArguments }.FilterNotNull().ToList();


        public abstract record BlendArguments : CommandArgument;

        public record StateArgument : BlendArguments {
            public CommandArgument stateArg { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new[]
                { stateArg };
        }

        public record SrcDstArguments : BlendArguments {
            public CommandArgument srcFactorArg { get; init; }
            public CommandArgument dstFactorArg { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new[]
                { srcFactorArg, dstFactorArg };
        }

        public record RgbaFactorArguments : BlendArguments {
            public CommandArgument srcRgbFactor   { get; init; }
            public CommandArgument srcAlphaFactor { get; init; }
            public CommaToken      commaToken     { get; init; } = new();
            public CommandArgument dstRgbFactor   { get; init; }
            public CommandArgument dstAlphaFactor { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens =>
                new SyntaxOrToken<Shaderlab>[]
                    { srcRgbFactor, srcAlphaFactor, commaToken, dstRgbFactor, dstAlphaFactor };
        }
    }
}
