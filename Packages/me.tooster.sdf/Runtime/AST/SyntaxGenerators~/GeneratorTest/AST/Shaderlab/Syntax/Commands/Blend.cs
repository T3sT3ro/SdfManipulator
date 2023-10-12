#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    /// <a href="https://docs.unity3d.com/Manual/SL-Blend.html">Blend</a>
    [AstSyntax] public partial record Blend : Command {
        public BlendKeyword     blendKeyword { get; init; } = new();
        public        CommandArgument? renderTarget { get; init; }
        public        BlendArguments   blendArguments { get; init; }


        [AstSyntax] public abstract partial record BlendArguments : CommandArgument;

        [AstSyntax] public partial record StateArgument : BlendArguments {
            public CommandArgument stateArg { get; init; }
        }

        [AstSyntax] public partial record SrcDstArguments : BlendArguments {
            public CommandArgument srcFactorArg { get; init; }
            public CommandArgument dstFactorArg { get; init; }
        }

        [AstSyntax] public partial record RgbaFactorArguments : BlendArguments {
            public        CommandArgument srcRgbFactor { get; init; }
            public        CommandArgument srcAlphaFactor { get; init; }
            public CommaToken      commaToken { get; init; } = new();
            public        CommandArgument dstRgbFactor { get; init; }
            public        CommandArgument dstAlphaFactor { get; init; }
        }
    }
}
