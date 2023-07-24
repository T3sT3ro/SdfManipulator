namespace AST.Shaderlab.Commands {
    public record BlendOp : Shader.SubShader.Command, Shader.SubShader.Pass.Command {
        public UnityEngine.Rendering.BlendOp operation { get; set; }
    }
}