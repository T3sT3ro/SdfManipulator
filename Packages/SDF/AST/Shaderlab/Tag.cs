#nullable enable
namespace AST.Shaderlab {
    /// <a href="https://docs.unity3d.com/Manual/SL-SubShaderTags.html">Tags</a>
    public interface Tag {
        public record Custom : Shader.SubShader.Tag, Shader.SubShader.Pass.Tag {
            public string key   { get; set; }
            public string value { get; set; }
        }

        public record Queue : Shader.SubShader.Tag, Shader.SubShader.Pass.Tag {
            public enum Name { BACKGROUND, GEOMETRY, ALPHA_TEST, TRANSPARENT, OVERLAY }

            public Name name   { get; set; }
            public int?  offset { get; set; }
        }
    }
}
