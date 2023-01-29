using UnityEditor;

namespace API {
    /// <summary>
    /// Something with internal and external representation
    /// FIXME: is this level of abstraction needed?
    /// </summary>
    public interface Representable {
        // used for generators
        public string InternalName { get; }
        // used for user-friendly display in GUI
        public string DisplayName  { get; }
    }
}
