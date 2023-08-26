using System;
using UnityEditor;
using UnityEngine;

namespace API {
    // TODO make it impossible to create propeties directly, only using graph creational methods

    /// <summary>
    /// A constant literal or a uniform, which can be exposed to outside world.
    /// They are like uniforms and blackboard properties in shader graph
    /// </summary>
    [Serializable]
    public abstract record Property(string InternalName, string DisplayName) : Representable {
        [field:SerializeField] public bool   Exposed        { get; set; } // determines if property is exported to material properties
        [field:SerializeField] public GUID   Guid           { get; } = GUID.Generate();
    }

    /// <summary>Typed property with possibility to export to external world</summary>
    /// <typeparam name="T">type that this property holds</typeparam>
    [Serializable]
    public record Property<T>: Property {
        [field:SerializeField] public T DefaultValue { get; internal set; }

        internal Property(string internalName, string displayName, T defaultValue) : base(internalName, displayName) {
            this.DefaultValue = defaultValue;
        }
    }
}
