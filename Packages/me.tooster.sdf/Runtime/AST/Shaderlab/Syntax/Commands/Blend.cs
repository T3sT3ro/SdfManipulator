#nullable enable
namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    /// <a href="https://docs.unity3d.com/Manual/SL-Blend.html">Blend</a>
    [SyntaxNode] public partial record Blend : Command {
        public BlendKeyword     blendKeyword   { get; init; } = new();
        public CommandArgument? renderTarget   { get; init; }
        public BlendArguments   blendArguments { get; init; }


        [SyntaxNode] public abstract partial record BlendArguments : CommandArgument;

        [SyntaxNode] public partial record StateArgument : BlendArguments {
            public CommandArgument stateArg { get; init; }
        }

        [SyntaxNode] public partial record SrcDstArguments : BlendArguments {
            public CommandArgument srcFactorArg { get; init; }
            public CommandArgument dstFactorArg { get; init; }
        }

        [SyntaxNode] public partial record RgbaFactorArguments : BlendArguments {
            public CommandArgument srcRgbFactor   { get; init; }
            public CommandArgument srcAlphaFactor { get; init; }
            public CommaToken      commaToken     { get; init; } = new();
            public CommandArgument dstRgbFactor   { get; init; }
            public CommandArgument dstAlphaFactor { get; init; }
        }
    }
}
