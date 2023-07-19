using API;
using UnityEngine;

namespace Nodes.MasterNodes {
    // TODO dynamic interpolators using InOutPorts
    public class BasicVertToFragNode : VertToFragNode {
        public string InternalName => "v2f_basic";
        public string DisplayName  => "Basic Vertex to Fragment";

        public InOutPort<Variable<Vector4>> position { get; }
        public InOutPort<Variable<Vector4>> normal   { get; }
        public InOutPort<Variable<Vector4>> texCoord { get; }
        public InOutPort<Variable<Vector4>> color    { get; }

        public BasicVertToFragNode() {
            position = new InOutPort<Variable<Vector4>>(this, "Vertex position");
            normal = new InOutPort<Variable<Vector4>>(this, "Vertex normal");
            texCoord = new InOutPort<Variable<Vector4>>(this, "Vertex texture coordinate");
            color = new InOutPort<Variable<Vector4>>(this, "Vertex color");
        }
    }
}
