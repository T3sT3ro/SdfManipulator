using System;
namespace me.tooster.sdf {
    public class ShaderGenerationException : Exception {
        public ShaderGenerationException(string message) : base(message) { }
        public ShaderGenerationException(string message, Exception inner) : base(message, inner) { }
    }
}
