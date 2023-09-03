#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Commands {
    /// <a href="https://docs.unity3d.com/Manual/SL-Blend.html">Blend</a>
    [Syntax] public partial record Blend : Command {
        [Init] private readonly BlendKeyword     _blendKeyword;
        private readonly        CommandArgument? _renderTarget;
        private readonly        BlendArguments   _blendArguments;


        [Syntax] public abstract partial record BlendArguments : CommandArgument;

        [Syntax] public partial record StateArgument : BlendArguments {
            private readonly CommandArgument _stateArg;
        }

        [Syntax] public partial record SrcDstArguments : BlendArguments {
            private readonly CommandArgument _srcFactorArg;
            private readonly CommandArgument _dstFactorArg;
        }

        [Syntax] public partial record RgbaFactorArguments : BlendArguments {
            private readonly        CommandArgument _srcRgbFactor;
            private readonly        CommandArgument _srcAlphaFactor;
            [Init] private readonly CommaToken      _commaToken;
            private readonly        CommandArgument _dstRgbFactor;
            private readonly        CommandArgument _dstAlphaFactor;
        }
    }
}
