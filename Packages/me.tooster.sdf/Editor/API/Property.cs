using System;
using UnityEngine;

namespace me.tooster.sdf.Editor.API {
    // TODO: replace with observable properties on types and property metadata (to avoid duplication and decouple concepts)
    // TODO: declare properties getter as dictionary of properties (in controllers). Consider removing internal name? Or not? (user defined name?)
    /// <summary>
    /// A wrapper around a 
    /// A constant literal or a uniform, which can be exposed to outside world.
    /// They are like uniforms and blackboard properties in shader graph
    /// </summary>
    public abstract class Property {
        [field: SerializeField] public bool IsExposed { get; set; }

        protected Property(string InternalName, string DisplayName) {
            this.InternalName = InternalName;
            this.DisplayName = DisplayName;
        }

        public delegate void ValueChangedEvent(Property caller, object newValue);

        public event ValueChangedEvent onValueChanged = delegate { };

        public string InternalName { get; init; }
        public string DisplayName  { get; init; }
    }

    /// <summary>Typed property with possibility to export to external world</summary>
    /// <typeparam name="T">type that this property holds</typeparam>
    public class Property<T> : Property {
        public Property(string InternalName, string DisplayName, T DefaultValue) : base(InternalName, DisplayName) =>
            this.DefaultValue = DefaultValue;

        [field: SerializeField] public T DefaultValue { get; internal set; }

        public new delegate void ValueChangedEvent(Property<T> caller, T newValue);

        public new event ValueChangedEvent onValueChanged = delegate { };
        //(caller, value) => Debug.Log($"value changed at {caller.DisplayName}:\n{value}");

        public void UpdateValue(Property<T> caller, T newValue) => onValueChanged(caller, newValue);
    }
}
