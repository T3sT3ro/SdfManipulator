#nullable enable
namespace AST.Shaderlab {
    public interface Command {
        // undocumented feature, the syntax is "COMMAND [_PROPERTY]" to get value from property
        public record Calculated : Shader.SubShader.Command, Shader.SubShader.Pass.Command {
            public Identifier id { get; set; }
        }

        public record ZClip : Shader.SubShader.Command, Shader.SubShader.Pass.Command {
            public bool enabled { get; set; }
        }

        public record ZTest : Shader.SubShader.Command, Shader.SubShader.Pass.Command {
            public UnityEngine.Rendering.CompareFunction operation { get; set; }
        }

        public record ZWrite : Shader.SubShader.Command, Shader.SubShader.Pass.Command {
            public bool enabled { get; set; }
        }

        /// <a href="https://docs.unity3d.com/Manual/SL-Blend.html">Blend</a>
        public record Blend : Shader.SubShader.Command, Shader.SubShader.Pass.Command {
            public EnabledState? enabledState { get; set; }

            public interface EnabledState {
                public UnityEngine.Rendering.BlendMode? srcFactor      { get; set; }
                public UnityEngine.Rendering.BlendMode? dstFactor      { get; set; }
                public UnityEngine.Rendering.BlendMode? srcAlphaFactor { get; set; }
                public UnityEngine.Rendering.BlendMode? dstAlphaFactor { get; set; }
            }
        }
        
        public record BlendOp : Shader.SubShader.Command, Shader.SubShader.Pass.Command {
            public UnityEngine.Rendering.BlendOp operation { get; set; }
        }
        
        public record Cull : Shader.SubShader.Command, Shader.SubShader.Pass.Command {
            public UnityEngine.Rendering.CullMode mode { get; set; }
        }
    }
}
