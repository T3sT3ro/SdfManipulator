namespace AST.Shaderlab.Commands {
    public record ZTest : Shader.SubShader.Command, Shader.SubShader.Pass.Command {
        public UnityEngine.Rendering.CompareFunction operation { get; set; }
    }
}