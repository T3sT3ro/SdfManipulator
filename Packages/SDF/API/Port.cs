using System.Collections.Generic;

namespace API {
    /// <summary>
    /// Abstraction over connections and their state changes. Typed ports are 
    /// </summary>
    public abstract class Port {
        public Node ContainingNode { get; }

        public string DisplayName { get; }

        // public bool Enabled        { get; set; }
        // public bool Visible        { get; set; }
        protected Port(Node containingNode, string displayName) {
            ContainingNode = containingNode;
            DisplayName = displayName;
        }
    }

    // needed for a generic collection of input/output ports without type parameters
    public abstract class InputPort : Port {
        public OutputPort ConnectedPort { get; protected set; } = null;

        protected InputPort(Node containingNode, string displayName) : base(containingNode, displayName) { }

        protected bool ConnectTo(OutputPort outputPort) {
            if (outputPort == ConnectedPort) return false;

            ConnectedPort = outputPort;
            return true;
        }
    }

    public abstract class OutputPort : Port {
        // public ISet<InputPort> ConnectedPorts { get; } = new HashSet<InputPort>();
        protected OutputPort(Node containingNode, string displayName) : base(containingNode, displayName) { }
    }

    public class InputPort<T> : InputPort {
        public InputPort(Node containingNode, string displayName) : base(containingNode, displayName) { }
    }

    public class OutputPort<T> : OutputPort {
        public OutputPort(Node containingNode, string displayName) : base(containingNode, displayName) { }
    }
}
