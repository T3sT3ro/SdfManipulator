using System.Collections.Generic;

namespace API {
    /// <summary>
    /// Abstraction over connections and their state changes. Typed ports are 
    /// </summary>
    public interface Port : Representable {
        public Node ContainingNode { get; }
        // public bool Enabled        { get; set; }
        // public bool Visible        { get; set; }
    }

    // needed for a generic collection of input/outpu ports without type parameters
    public abstract class InputPort : Port {
        public abstract string InternalName   { get; }
        public abstract string DisplayName    { get; }
        public abstract Node   ContainingNode { get; }
    }

    public abstract class OutputPort : Port {
        public abstract string InternalName   { get; }
        public abstract string DisplayName    { get; }
        public abstract Node   ContainingNode { get; }
    }

    public abstract class InputPort<T> : InputPort {
        public InputPort<T> ConnectedPort => null;
        public bool         IsConnected   => ConnectedPort != null;
    }

    public abstract class OutputPort<T> : OutputPort {
        public ISet<InputPort<T>> ConnectedPorts { get; } = new HashSet<InputPort<T>>();
    }
}
