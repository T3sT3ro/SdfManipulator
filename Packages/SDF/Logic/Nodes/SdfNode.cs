using API;
using UnityEngine;

namespace Nodes {
    public abstract class SdfNode : ConsumerNode, ProducerNode {
        public abstract string InternalName { get; }
        public abstract string DisplayName  { get; }

        public InputPort<Variable<Vector3>.Evaluator> SamplePoint { get; }

        public OutputPort<Variable<float>.Evaluator> Distance { get; }
        public OutputPort<Evaluator>                 Sdf      { get; }

        public delegate string Evaluator(Variable<Vector3>.Evaluator point);
    }
}
