using API;
using UnityEngine;

namespace Nodes.MasterNodes {
    public class UnlitFragOutNode : FragOutNode {
        public string InternalName => "frag_out";
        public string DisplayName  => "Fragment Output";

        public InputPort<Variable<Vector4>.Evaluator> Color { get; }

        public UnlitFragOutNode() {
            Color = new InputPort<Variable<Vector4>.Evaluator>(this, "Fragment color");
        }

        public delegate string Evaluator(Variable<Vector4>.Evaluator color);
    }
}
