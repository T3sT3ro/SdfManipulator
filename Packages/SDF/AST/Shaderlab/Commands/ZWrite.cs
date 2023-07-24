namespace AST.Shaderlab.Commands {
    public record ZWrite : Shader.SubShader.Command, Shader.SubShader.Pass.Command {
        public bool enabled { get; set; }
    }
}