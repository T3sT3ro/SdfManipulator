#nullable enable
namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    /// Blend [renderTarget] (<see cref="StateArgument"/> | <see cref="SrcDstArguments"/> | <see cref="RgbaFactorArguments"/>);
    /// <a href="https://docs.unity3d.com/Manual/SL-Blend.html">Blend</a>
    [SyntaxNode] public partial record Blend : Command {
        public BlendKeyword     blendKeyword   { get; init; } = new();
        public CommandArgument? renderTarget   { get; init; }
        public BlendArguments   blendArguments { get; init; }


        [SyntaxNode] public abstract partial record BlendArguments : CommandArgument;

        /// Off
        [SyntaxNode] public partial record StateArgument : BlendArguments {
            public CommandArgument stateArg { get; init; } = new OffKeyword();
        }

        /// (Source factor) (Dst factor)
        [SyntaxNode] public partial record SrcDstArguments : BlendArguments {
            public CommandArgument srcFactorArg { get; init; } = new SrcAlphaKeyword();
            public CommandArgument dstFactorArg { get; init; } = new DstAlphaKeyword();
        }

        /// (src factor rgb) (dst factor rgb), (src factor alpha) (dst factor alpha)
        [SyntaxNode] public partial record RgbaFactorArguments : BlendArguments {
            public CommandArgument srcRgbFactor   { get; init; }
            public CommandArgument srcAlphaFactor { get; init; }
            public CommaToken      commaToken     { get; init; } = new();
            public CommandArgument dstRgbFactor   { get; init; }
            public CommandArgument dstAlphaFactor { get; init; }
        }
    }
}
