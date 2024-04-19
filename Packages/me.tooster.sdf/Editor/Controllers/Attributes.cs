using System;
namespace me.tooster.sdf.Editor.Controllers {
    /**
     * Shader properties are used to expose properties as uniforms and material properties in the shader
     */
    [AttributeUsage(AttributeTargets.Property)]
    public class ShaderPropertyAttribute : Attribute {
        public string Description { get; set; }
    }



    /**
     * Structural properties change the structure of the shader AST and require regeneration of the shader code.
     */
    [AttributeUsage(AttributeTargets.Property)]
    public class ShaderStructuralAttribute : Attribute { }
}
