using System.Collections.Generic;
using API;

namespace Logic.Nodes {
    public abstract class PropertyNode : ProducerNode {
        public abstract string                InternalName { get; }
        public abstract string                DisplayName  { get; }
        public abstract ISet<OutputPort>      OutputPorts  { get; }
        public          bool                  External     { get; protected set; }
        public abstract IEnumerable<Property> GetProperties();
    }

    /// <summary>
    /// A Property node exposes shader variables to outside world.
    /// </summary>
    /// <typeparam name="T">Property type</typeparam>
    public class PropertyNode<T> : PropertyNode {
        public override string           InternalName { get; }
        public override string           DisplayName  { get; }
        public override ISet<OutputPort> OutputPorts  { get; }
        private         Property<T>      Property     { get; }

        private OutputPort<T> outputType;

        public override IEnumerable<Property> GetProperties() { yield return Property; }

        // protected OutputPort<T>

        public PropertyNode(string internalName, string displayName, bool external, Property<T> property) {
            InternalName = internalName;
            DisplayName = displayName;
            External = external;
            Property = property;
            outputType = new OutputPort<T>(this, "property");
        }
    }
}
