namespace AST.Shaderlab.Commands {
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
}