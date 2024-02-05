using System.Collections.Generic;

namespace me.tooster.sdf.Editor.API {
    public interface IShaderPartialProvider {
        public IEnumerable<Property> Properties { get; }
    }
}
