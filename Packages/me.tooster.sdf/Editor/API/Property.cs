using System;
using System.Runtime.CompilerServices;
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
        public bool   IsExposed    { get; set; }
        public string InternalName { get; init; }
        public string DisplayName  { get; init; }

        protected Property(string InternalName, string DisplayName) {
            this.InternalName = InternalName;
            this.DisplayName = DisplayName;
        }

        public abstract object CurrentValue { get; }

        public override string ToString() => $"{DisplayName} ({InternalName}<{CurrentValue.GetType().FullName}>: {CurrentValue})";
    }

    /// <summary>Typed property with possibility to export to external world</summary>
    /// <typeparam name="T">type that this property holds</typeparam>
    public class Property<T> : Property {
        public          T      DefaultValue { get; private set; }
        public          T      Value        { get; set; }
        public override object CurrentValue => Value;


        public Property(string InternalName, string DisplayName, T DefaultValue) : base(InternalName, DisplayName) =>
            this.DefaultValue = Value = DefaultValue;
    }
}
