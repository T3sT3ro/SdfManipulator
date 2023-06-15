using API;
using UnityEngine;

namespace Nodes.SdfNodes {
    public class SdfSphereNode : SdfNode {
        private         Variable<float> radius;
        public override string          InternalName => "sdf_sphere";
        public override string          DisplayName  => "SDF Sphere";

        public InputPort<Variable<Vector3>.Evaluator> Center { get; }
        public InputPort<Variable<Vector3>.Evaluator> Radius { get; }

        public OutputPort<Evaluator>               Sdf      { get; }
        public OutputPort<Variable<int>.Evaluator> Distance { get; }
    }
}
