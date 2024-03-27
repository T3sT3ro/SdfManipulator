using System;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    public class SdfException : Exception {
        public SdfException() { }

        public SdfException(string message) : base(message) { }

        public SdfException(string message, Exception inner) : base(message, inner) { }
    }
}
