namespace AST.Shaderlab.Commands {
    public record ZClip : Shader.SubShader.Command, Shader.SubShader.Pass.Command {
        public bool enabled { get; set; }
    }
}