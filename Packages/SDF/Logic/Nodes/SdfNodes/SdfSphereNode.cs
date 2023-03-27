using API;
using UnityEngine;

namespace Logic.Nodes.SdfNodes {
    public class SdfSphereNode : SdfNode {
        private         Property<float> radius;
        public override string          InternalName => "sdf_sphere";
        public override string          DisplayName  => "SDF Sphere";
    }
}
