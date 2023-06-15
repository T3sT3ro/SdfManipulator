using UnityEditor;

namespace API {
    /// <summary>
    /// A constant literal or a uniform. Can be exposed to outside world.
    /// Variables are like uniforms and blackboard properties in shader graph
    /// </summary>
    public interface Variable : Representable {
        public bool Exposed { get; set; } // if property is connected to a 
        public GUID Guid     { get; }
    }

    /// <summary>Typed variable with possibility to export to external world</summary>
    /// <typeparam name="T">type that this property holds</typeparam>
    public class Variable<T> : Variable {
        public GUID   Guid         { get; private set; }
        public T      DefaultValue { get; }
        public string InternalName { get; }
        public string DisplayName  { get; }
        public bool   Exposed     { get; set; }


        public Variable(string internalName, string displayName, T defaultValue) {
            Guid = GUID.Generate(); 
            InternalName = internalName;
            DisplayName = displayName;
            DefaultValue = defaultValue;
        }

        // fixme: consider moving to logic?
        public delegate string Evaluator();
    }
}
