using System;
using UnityEngine;

namespace me.tooster.sdf.Editor.API {
    /// <summary>
    /// A wrapper around a 
    /// A constant literal or a uniform, which can be exposed to outside world.
    /// They are like uniforms and blackboard properties in shader graph
    /// </summary>
    [Serializable]
    public abstract class Property {
        [field: SerializeField] public bool Exposed { get; set; } // is property a uniform or a constant?

        protected Property(string InternalName, string DisplayName) {
            this.InternalName = InternalName;
            this.DisplayName = DisplayName;
        }

        public string InternalName { private get; init; }
        public string DisplayName  { get;         init; }
    }

    /// <summary>Typed property with possibility to export to external world</summary>
    /// <typeparam name="T">type that this property holds</typeparam>
    [Serializable]
    public class Property<T> : Property {
        public Property(string InternalName, string DisplayName, T DefaultValue) : base(InternalName, DisplayName) {
            this.DefaultValue = DefaultValue;
        }

        [field: SerializeField] public T DefaultValue { get; internal set; }

        public delegate void ValueChangedEvent(object context, T newValue);

        public event ValueChangedEvent onValueChanged = delegate { };

        public void UpdateValue(object context, T newValue) => onValueChanged(context, newValue);
    }

    public interface IPropertyIdentifierProvider {
        public string GetIdentifier(Property property);
    }
}
