using API;
using UnityEngine;

namespace Nodes.MasterNodes {
    public class VertexInNode : VertInNode {
        public string InternalName => "v_in";
        public string DisplayName  => "Vertex Input";

        OutputPort<Variable<Vector4>.Evaluator> Position { get; }

        public VertexInNode() {
            Position = new OutputPort<Variable<Vector4>.Evaluator>(this, "Vertex position");
        }
    }
}
