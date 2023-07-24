using AST.Hlsl.Syntax;

namespace AST.Shaderlab.Commands {
    public record Calculated : Shader.SubShader.Command, Shader.SubShader.Pass.Command {
        public IdentifierName id { get; set; }
    }
}