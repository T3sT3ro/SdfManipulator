using API;
using UnityEngine;

namespace Nodes.MasterNodes {
    // TODO dynamic interpolators using InOutPorts
    public class BasicVertToFragNode : VertToFragNode {
        public string InternalName => "v2f_basic";
        public string DisplayName  => "Basic Vertex to Fragment";

        public delegate string Evaluator();

        public InOutPort<Variable<Vector4>.Evaluator> Position { get; }
        public InOutPort<Variable<Vector4>.Evaluator> Normal   { get; }
        public InOutPort<Variable<Vector4>.Evaluator> TexCoord { get; }
        public InOutPort<Variable<Vector4>.Evaluator> Color    { get; }

        public BasicVertToFragNode() {
            Position = new InOutPort<Variable<Vector4>.Evaluator>(this, "Vertex position");
            Normal = new InOutPort<Variable<Vector4>.Evaluator>(this, "Vertex normal");
            TexCoord = new InOutPort<Variable<Vector4>.Evaluator>(this, "Vertex texture coordinate");
            Color = new InOutPort<Variable<Vector4>.Evaluator>(this, "Vertex color");
        }
    }
}
