using API;

namespace Nodes {
    // FIXME: refactor to "ValueNode" with support for constants?
    public abstract class VariableNode : ProducerNode {
        public abstract string InternalName { get; }
        public abstract string DisplayName  { get; }
        public          bool   Exposed      { get; protected set; }
    }

    /// <summary>
    /// A Property node exposes shader variables to outside world.
    /// </summary>
    /// <typeparam name="T">Property type</typeparam>
    public class VariableNode<T> : VariableNode {
        public override string      InternalName { get; }
        public override string      DisplayName  { get; }
        public          Variable<T> Variable     { get; }

        public OutputPort<Variable<T>> Value { get; }

        public VariableNode(string internalName, string displayName, bool exposed, Variable<T> variable) {
            InternalName = internalName;
            DisplayName = displayName;
            Exposed = exposed;
            Variable = variable;
            Value = new OutputPort<Variable<T>>(this, "value");
        }
    }
}
