#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using AST.Hlsl;
using AST.Shaderlab.Syntax;
using AST.Shaderlab.Syntax.Commands;
using AST.Syntax;

namespace AST.Shaderlab.Syntax.Commands {
    /// <a href="https://docs.unity3d.com/Manual/SL-Blend.html">Blend</a>
    public record Blend : Command {
        public BlendKeyword     blendKeyword   { get; set; } = new();
        public CommandArgument? renderTarget   { get; set; }
        public BlendArguments   blendArguments { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
                { blendKeyword }
            .AppendNotNull(renderTarget)
            .Append(blendArguments)
            .ToList();


        public abstract record BlendArguments : CommandArgument;

        public record StateArgument : BlendArguments {
            public CommandArgument stateArg { get; set; }

            public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new[] { stateArg };
        }

        public record SrcDstArguments : BlendArguments {
            public CommandArgument srcFactorArg { get; set; }
            public CommandArgument dstFactorArg { get; set; }

            public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new[]
                { srcFactorArg, dstFactorArg };
        }

        public record RgbaFactorArguments : BlendArguments {
            public CommandArgument srcRgbFactor   { get; set; }
            public CommandArgument srcAlphaFactor { get; set; }
            public CommaToken      commaToken     { get; set; } = new();
            public CommandArgument dstRgbFactor   { get; set; }
            public CommandArgument dstAlphaFactor { get; set; }

            public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => new IShaderlabSyntaxOrToken[]
                { srcRgbFactor, srcAlphaFactor, commaToken, dstRgbFactor, dstAlphaFactor };
        }
    }
}
