using API;
using PortData;
using UnityEngine;

namespace Nodes.SdfNodes {
    public class SdfSphereNode : SdfNode {
        private Variable<float> _radius;

        public override string InternalName => "sdf_sphere";
        public override string DisplayName  => "SDF Sphere";

        public InputPort<Variable<Vector3>> center { get; }
        public InputPort<Variable<Vector3>> radius { get; }

        public OutputPort<SdfFunction>   sdf      { get; }
        public OutputPort<Variable<int>> distance { get; }
    }
}
