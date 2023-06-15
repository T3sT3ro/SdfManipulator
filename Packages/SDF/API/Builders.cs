namespace API {

    public interface NodeBuilder { }
    public abstract class NodeBuilder<NodeT> : NodeBuilder where NodeT : Node {
        private NodeT node;

        protected NodeBuilder(NodeT node) => this.node = node;
    }

    public interface GraphBuilder { }
    public abstract class AbstractGraphBuilder : GraphBuilder {
        private Graph graph;
        protected AbstractGraphBuilder(Graph graph) => this.graph = graph;
    }

}
