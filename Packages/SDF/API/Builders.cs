using System.Collections.Generic;

namespace API {

    public interface NodeBuilder {
        
        public delegate string Selector(string value); // selectors extract data from evaluation result, e.g. "output.color"
        public Selector GetSelectorFor(InputPort port);
    }
    public abstract class NodeBuilder<NodeT> : NodeBuilder where NodeT : Node {
        protected NodeT node;

        protected NodeBuilder(NodeT node) => this.node = node;

        
        public delegate string GlobalDefinition();

        public virtual NodeBuilder.Selector GetSelectorFor(InputPort port) => _ => "";
    }

    public interface GraphBuilder { }
    public abstract class AbstractGraphBuilder : GraphBuilder {
        private Graph graph;
        protected AbstractGraphBuilder(Graph graph) => this.graph = graph;
    }

}
