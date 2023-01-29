using System.Collections.Generic;
using API;

namespace Logic.Nodes {
    public abstract class PropertyNode : ProducerNode {
        public abstract string           InternalName { get; }
        public abstract string           DisplayName  { get; }
        public abstract ISet<OutputPort> OutputPorts  { get; }
        public          bool             External     { get; protected set; }

        #region selective visitor pattern

        public interface Visitor<out R> {
            public R visit(PropertyNode node);
        }

        public R accept<R>(Visitor<R>      visitor) => visitor.visit(this);
        public R accept<R>(Node.Visitor<R> visitor) => accept((Visitor<R>)visitor);

        #endregion
    }

    /// <summary>
    /// A Property node exposes shader variables to outside world.
    /// </summary>
    /// <typeparam name="T">Property type</typeparam>
    public abstract class PropertyNode<T> : PropertyNode where T : Property {
        public override string InternalName { get; }
        public override string DisplayName  { get; }
        private         T      Property     { get; }
        

        public PropertyNode(string internalName, string displayName, bool external, T property) {
            InternalName = internalName;
            DisplayName  = displayName;
            External     = external;
            Property     = property;
        }
    }
}
