#nullable enable
using System;
using System.Collections.Generic;

namespace me.tooster.sdf.Editor.API {
    #region interfaces

    // TODO: Ports shouldn't have any means of connecting to other ports, it should be done by Graph
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
    public interface IInputPort : Port {
        // non nullable, all input ports must have a connection, even if it's a default value
        public IOutputPort Source { get; }
    }

    public interface IOutputPort : Port {
        public IReadOnlyCollection<IInputPort> Targets { get; }
    }


    // TODO: consider adding optional inputs? ShaderGRaph has it for example in NormalFromTexture and SamplerState input
    public interface IInputPort<T> : IInputPort where T : Port.Data {
        new IOutputPort<T>     Source { get; }
        IOutputPort IInputPort.Source => Source;

        // used by Graph to connect ports
        public void UnsafeSetSource(IOutputPort<T> source);

        public T Eval();
    }

    public interface IOutputPort<T> : IOutputPort where T : Port.Data {
        public new IReadOnlyCollection<IInputPort<T>> Targets { get; }
        IReadOnlyCollection<IInputPort> IOutputPort.  Targets => Targets;

        // used by Graph to connect ports
        public void UnsafeAddTarget(IInputPort<T>    target);
        public void UnsafeRemoveTarget(IInputPort<T> target);

        public T Eval();
    }

    #endregion

    #region implementation

    public abstract class AbstractPort : Port {
        public Node   Node        { get; }
        public string DisplayName { get; }

        protected AbstractPort(Node node, string displayName) {
            Node = node;
            DisplayName = displayName;
        }
    }

    [Serializable]
    internal class InputPort<T> : AbstractPort, IInputPort<T> where T : Port.Data {
        internal IOutputPort<T> _connectedOutput; // raw internal value accessible by Graph

        public IOutputPort<T>  Source => _connectedOutput;
        IOutputPort IInputPort.Source => Source;

        public void UnsafeSetSource(IOutputPort<T> source) => _connectedOutput = source;

        public T Eval() => Source.Eval();

        public InputPort(Node node, string displayName, IOutputPort<T> source) : base(node, displayName) {
            this._connectedOutput = source;
        }
    }

    [Serializable]
    internal class OutputPort<T> : AbstractPort, IOutputPort<T> where T : Port.Data {
        // for raw access in this package
        internal readonly HashSet<IInputPort<T>> _targets = new HashSet<IInputPort<T>>();

        public IReadOnlyCollection<IInputPort<T>>   Targets => _targets;
        IReadOnlyCollection<IInputPort> IOutputPort.Targets => Targets;

        private event Func<T> evaluator;

        public void UnsafeAddTarget(IInputPort<T>    target) => _targets.Add(target);
        public void UnsafeRemoveTarget(IInputPort<T> target) => _targets.Remove(target);

        public OutputPort(Node node, string displayName, Func<T> evaluator) : base(node, displayName) {
            this.evaluator = evaluator;
        }

        public T Eval() => evaluator.Invoke();
    }

    // FIXME: isn't it messy, complicated? Use a virtual (invisible) node instead of this?
    public class InOutPort<T> : AbstractPort, IInputPort<T>, IOutputPort<T> where T : Port.Data {
        // transform translates output from input port to output port
        public InOutPort(Node node, string displayName, IOutputPort<T> source, Func<T, T>? transform = null) : base(
            node,
            displayName) {
            Out = new OutputPort<T>(node, displayName, transform == null ? Eval : () => transform(Eval()));
            In = new InputPort<T>(node, displayName, source);
        }

        public IInputPort<T>  In  { get; }
        public IOutputPort<T> Out { get; }

        public void UnsafeSetSource(IOutputPort<T>   source) => In.UnsafeSetSource(source);
        public void UnsafeAddTarget(IInputPort<T>    target) => Out.UnsafeAddTarget(target);
        public void UnsafeRemoveTarget(IInputPort<T> target) => Out.UnsafeRemoveTarget(target);


        public IOutputPort<T>  Source => In.Source;
        IOutputPort IInputPort.Source => Source;

        public IReadOnlyCollection<IInputPort<T>>   Targets => Out.Targets;
        IReadOnlyCollection<IInputPort> IOutputPort.Targets => Targets;

        public T Eval() => In.Eval();
    }

    #endregion
}
