#nullable enable
using System;
using System.Collections.Generic;

namespace API {
    /// <summary>
    /// Abstraction over connections and their state changes. Typed ports are 
    /// </summary>
    public interface Port {
        public Node   Node        { get; }
        public string DisplayName { get; }

        // public bool Enabled        { get; set; }
        // public bool Visible        { get; set; }

        /// <summary>
        /// It's a convenient wrapper for data passed between ports.
        /// A port data type represents a single logical concept (for example "something that gives a scalar value")
        /// and provides all necessarry info to use that data at input site.
        /// </summary>
        public abstract record Data;
    }

    // base class used in enumerations and alike
    public interface InputPort : Port {
        // non nullable, all input ports must have a connection, even if it's a default value
        public OutputPort ConnectedOutput { get; }
    }

    public interface OutputPort : Port {
        public IReadOnlyCollection<InputPort> ConnectedInputs { get; }
    }


    public abstract class AbstractPort : Port {
        public Node   Node        { get; }
        public string DisplayName { get; }

        protected AbstractPort(Node node, string displayName) {
            Node = node;
            DisplayName = displayName;
        }
    }

    public class InputPort<T> : AbstractPort, InputPort where T : Port.Data {
        /*
         * fixme? are these convenience methods even needed?
         * maybe they should be handled in the Graph class as a view into collection of nodes
         */
        public OutputPort<T> ConnectedOutput { get; internal set; }
        OutputPort InputPort.ConnectedOutput => ConnectedOutput;

        public T Eval() => ConnectedOutput.Eval();

        public InputPort(Node node, string displayName, OutputPort<T> outputPort) : base(node, displayName) {
            this.ConnectedOutput = outputPort;
            outputPort.connections.Add(this);
        }
    }

    public class OutputPort<T> : AbstractPort, OutputPort where T : Port.Data {
        internal readonly HashSet<InputPort<T>>             connections = new HashSet<InputPort<T>>();
        public            IReadOnlyCollection<InputPort<T>> ConnectedInputs => connections;
        IReadOnlyCollection<InputPort> OutputPort.          ConnectedInputs => ConnectedInputs;
        private event Func<T>                               evaluator;


        public OutputPort(Node node, string displayName, Func<T> evaluator) : base(node, displayName) {
            this.evaluator = evaluator;
        }

        public T Eval() => evaluator.Invoke();
    }

    public class InOutPort<T> : AbstractPort, InputPort, OutputPort where T : Port.Data {
        // transform translates output from input port to output port
        public InOutPort(Node node, string displayName, Func<T, T>? transform = null) : base(node, displayName) {
            Out = new OutputPort<T>(node, displayName, transform == null ? Eval : () => transform(Eval()));
            In = new InputPort<T>(node, displayName, Out);
        }

        public InputPort<T>  In  { get; }
        public OutputPort<T> Out { get; }

        public OutputPort                     ConnectedOutput => In.ConnectedOutput;
        public IReadOnlyCollection<InputPort> ConnectedInputs => Out.ConnectedInputs;

        public T Eval() => In.Eval();
    }
}
