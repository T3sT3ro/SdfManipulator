using System;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor.API {
    // TODO make it impossible to create propeties directly, only using graph creational methods

    /// <summary>
    /// A constant literal or a uniform, which can be exposed to outside world.
    /// They are like uniforms and blackboard properties in shader graph
    /// </summary>
    [Serializable]
    public abstract record Property(string InternalName, string DisplayName) : Representable {
        [field:SerializeField] public bool   Exposed        { get; set; } // determines if property is exported to material properties
        [field:SerializeField] public Guid   Guid           { get; } = Guid.NewGuid();
    }

    /// <summary>Typed property with possibility to export to external world</summary>
    /// <typeparam name="T">type that this property holds</typeparam>
    [Serializable]
    public record Property<T>(string InternalName, string DisplayName, T DefaultValue)
        : Property(InternalName, DisplayName) {
        [field:SerializeField] public T DefaultValue { get; internal set; } = DefaultValue;
    }
}
