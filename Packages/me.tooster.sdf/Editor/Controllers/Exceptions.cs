using System;
namespace me.tooster.sdf.Editor.Controllers {
    public class ShaderGenerationException : Exception {
        public ShaderGenerationException(string message) : base(message) { }
        public ShaderGenerationException(string message, Exception inner) : base(message, inner) { }
    }
}
