namespace API {
    /// <summary>
    /// An interface for communicating with an external world.
    /// Properties are for example uniforms.
    /// They are like a blackboard properties in shader graph
    /// </summary>
    public interface Property : Representable {
        public bool External { get; set; } // if property is connected to a 

        #region selective visitor pattern

        public interface Visitor<out R> {
            public R visit(Property property);
        }

        public R accept<R>(Visitor<R> visitor);

        #endregion
    }

    /// <summary>
    /// Typed property
    /// </summary>
    /// <typeparam name="T">type that this property holds</typeparam>
    public abstract class Property<T> : Property {
        public T      Value        { get; set; }
        public T      DefaultValue { get; }
        public string InternalName { get; }
        public string DisplayName  { get; }
        public bool   External     { get; set; }


        public Property(string internalName, string displayName, T defaultValue) {
            InternalName = internalName;
            DisplayName  = displayName;
            Value        = DefaultValue = defaultValue;
        }


        #region selective visitor pattern

        public abstract R accept<R>(Property.Visitor<R> visitor);

        #endregion
    }
}
