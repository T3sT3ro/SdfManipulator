using System.Collections.Generic;

// marker interfaces
namespace API {
    /// 
    public interface VertInNode : ProducerNode {
        public delegate string Evaluator();
    }
    
    /// Evaluates output from vertex and input to fragment,
    /// defines interpolators
    public interface VertToFragNode : ConsumerNode, ProducerNode {
        // TODO: passes, SubShaders, tags, LODS
        public delegate string FragmentEvaluator();
        public delegate string VertexEvaluator();
    }

    /// Evaluates output from fragment shader
    public interface FragOutNode : ConsumerNode {
        public delegate string Evaluator();
    }
}
