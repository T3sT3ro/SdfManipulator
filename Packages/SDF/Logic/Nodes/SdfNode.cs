using API;
using PortData;
using UnityEngine;

namespace Nodes {
    public abstract class SdfNode : ConsumerNode, ProducerNode {
        public abstract string InternalName { get; }
        public abstract string DisplayName  { get; }

        public InputPort<Variable<Vector3>> SamplePoint { get; }

        public OutputPort<Variable<float>> Distance { get; }
        public OutputPort<SdfFunction>       Sdf      { get; }
    }
}
