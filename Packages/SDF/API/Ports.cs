using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace API {
    /// <summary>
    /// Abstraction over connections and their state changes. Typed ports are 
    /// </summary>
    public interface Port {
        public Node   Node        { get; }
        public string DisplayName { get; }

        // public bool Enabled        { get; set; }
        // public bool Visible        { get; set; }
    }

    public abstract class AbstractPort : Port {
        public Node   Node        { get; }
        public string DisplayName { get; }

        protected AbstractPort(Node node, string displayName) {
            Node = node;
            DisplayName = displayName;
        }
    }

    // needed for a generic collection of input/output ports without type parameters
    public interface InputPort : Port {
        public OutputPort IncomingConnection { get; }
    }

    public interface OutputPort : Port {
        public IReadOnlyCollection<InputPort> OutgoingConnections { get; }
    }

    public class InputPort<T> : AbstractPort, InputPort where T : Delegate {
        public InputPort(Node node, string displayName) : base(node, displayName) { }

        public OutputPort IncomingConnection { get; internal set; }
    }

    public class OutputPort<T> : AbstractPort, OutputPort where T : Delegate {
        public OutputPort(Node node, string displayName) : base(node, displayName) { }

        public IReadOnlyCollection<InputPort> OutgoingConnections { get; } = new HashSet<InputPort>();

        public void ConnectTo([NotNull] InputPort<T> inputPort) {
            if (inputPort == null) throw new ArgumentNullException(nameof(inputPort));

            if (inputPort.IncomingConnection == this) return;

            inputPort.IncomingConnection = this;
            ((HashSet<InputPort>)this.OutgoingConnections).Add(inputPort);
        }

        public void DisconnectFrom([NotNull] in InputPort<T> inputPort) {
            if (inputPort == null) throw new ArgumentNullException(nameof(inputPort));

            if (inputPort.IncomingConnection != this) return;

            inputPort.IncomingConnection = null;
            ((HashSet<InputPort>)this.OutgoingConnections).Remove(inputPort);
        }
    }

    public class InOutPort<T> : AbstractPort, InputPort, OutputPort where T : Delegate {
        
        private InputPort<T>  inputPort;
        private OutputPort<T> outputPort;

        public OutputPort                     IncomingConnection  => inputPort.IncomingConnection;
        public IReadOnlyCollection<InputPort> OutgoingConnections => outputPort.OutgoingConnections;

        public InOutPort(Node node, string displayName) : base(node, displayName) {}
    }
}
