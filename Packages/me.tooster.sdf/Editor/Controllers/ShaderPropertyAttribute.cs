using System;
namespace me.tooster.sdf.Editor.Controllers {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ShaderPropertyAttribute : Attribute {
        public string Description { get; set; }
        public bool   Structural  { get; set; }

        public ShaderPropertyAttribute(string description, bool structural = false) {
            Description = description;
            Structural = structural;
        }
    }
}
