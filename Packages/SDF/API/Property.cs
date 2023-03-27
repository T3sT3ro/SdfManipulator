namespace API {
    /// <summary>
    /// An interface for communicating with an external world.
    /// Properties are for example uniforms.
    /// They are like a blackboard properties in shader graph
    /// </summary>
    public interface Property : Representable {
        public bool External { get; set; } // if property is connected to a 
    }

    /// <summary>
    /// Typed property
    /// </summary>
    /// <typeparam name="T">type that this property holds</typeparam>
    public class Property<T> : Property{
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
    }
}
