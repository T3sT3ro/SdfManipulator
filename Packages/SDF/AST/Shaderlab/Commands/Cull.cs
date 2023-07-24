#nullable enable
namespace AST.Shaderlab.Commands {
    public record Cull : Shader.SubShader.Command, Shader.SubShader.Pass.Command {
        public UnityEngine.Rendering.CullMode mode { get; set; }
    }
}
