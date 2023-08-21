using UnityEditor;

namespace API {
    // TODO make it impossible to create propeties directly, only using graph creational methods
    
    /// <summary>
    /// A constant literal or a uniform, which can be exposed to outside world.
    /// They are like uniforms and blackboard properties in shader graph
    /// </summary>
    public abstract record Property(string InternalName, string DisplayName) : Representable {
        public bool Exposed { get; set; } // determines if property is exported to material properties
        public GUID Guid    { get; private set; } = GUID.Generate();
    }

    /// <summary>Typed property with possibility to export to external world</summary>
    /// <typeparam name="T">type that this property holds</typeparam>
    public record Property<T>(string InternalName, string DisplayName, T DefaultValue)
        : Property(InternalName, DisplayName);
}
