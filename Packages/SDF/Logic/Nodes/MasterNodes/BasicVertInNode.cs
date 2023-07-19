using API;
using UnityEngine;

namespace Nodes.MasterNodes {
    public class VertexInNode : VertInNode {
        public string InternalName => "v_in";
        public string DisplayName  => "Vertex Input";

        OutputPort<Variable<Vector4>> position { get; }

        public VertexInNode() { position = new OutputPort<Variable<Vector4>>(this, "Vertex position"); }
    }
}
