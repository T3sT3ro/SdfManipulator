using System;

namespace me.tooster.sdf.Editor.API {
    // attribute for node builder to get a list of strings representing the includes
    public class ShaderIncludeAttribute : Attribute {
        public string[] ShaderIncludes { get; }
        public ShaderIncludeAttribute(params string[] shaderIncludes) { ShaderIncludes = shaderIncludes; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ShaderGlobalAttribute : Attribute {
        public ShaderGlobalAttribute() {}
    }
}