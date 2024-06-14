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



    /// <summary>
    /// When added to a property, the auto generated property field will be shown only when the enum value of specified bcking field matches one of the values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class VisibleWhenAttribute : Attribute {
        public readonly string enumBackingField;
        public readonly int[]  enumValues;

        public VisibleWhenAttribute(string enumBackingField, params int[] enumValues) {
            this.enumBackingField = enumBackingField;
            this.enumValues = enumValues;
        }
    }
}
