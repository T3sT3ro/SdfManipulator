using System;
namespace me.tooster.sdf.Editor.Controllers {
    // attribute for node builder to get a list of strings representing the includes
    [Obsolete("Use subtype of Data.Requierments instead")]
    public class ShaderIncludeAttribute : Attribute {
        public ShaderIncludeAttribute(params string[] shaderIncludes) => ShaderIncludes = shaderIncludes;
        public string[] ShaderIncludes { get; }
    }



    /**
     * Shader properties expose property as uniform and as material property in the shader
     */
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShaderPropertyAttribute : Attribute {
        public string Description { get; set; }
    }



    /**
     * Structural properties change the structure of the shader AST and require regeneration of the shader code.
     */
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShaderStructuralAttribute : Attribute { }
}
