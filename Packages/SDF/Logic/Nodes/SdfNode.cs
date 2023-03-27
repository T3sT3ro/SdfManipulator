using System.Collections.Generic;
using API;
using UnityEngine;

namespace Logic.Nodes {
    public abstract class SdfNode : ConsumerNode, ProducerNode {
        public abstract string           InternalName { get; }
        public abstract string           DisplayName  { get; }

        private InputPort<Vector3> positionPoint;
        private OutputPort<int>    distancePort;

        public          ISet<InputPort>  InputPorts   => new HashSet<InputPort> { positionPoint };
        public          ISet<OutputPort> OutputPorts  => new HashSet<OutputPort> { distancePort };
    }
}
