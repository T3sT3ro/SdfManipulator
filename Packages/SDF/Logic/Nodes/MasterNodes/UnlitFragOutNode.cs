using API;
using UnityEngine;

namespace Nodes.MasterNodes {
    public class UnlitFragOutNode : FragOutNode {
        public string InternalName => "frag_out";
        public string DisplayName  => "Fragment Output";

        public InputPort<Variable<Vector4>> color { get; }

        public UnlitFragOutNode() { color = new InputPort<Variable<Vector4>>(this, "Fragment color"); }
        //
        // public delegate string Evaluator(Variable<Vector4>.Evaluator color);
    }
}
