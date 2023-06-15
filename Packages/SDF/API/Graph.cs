using System.Collections.Generic;

namespace API {
    public interface Graph {
        VertInNode     VertIn     { get; }
        VertToFragNode VertToFrag { get; }
        FragOutNode    FragOut    { get; }
        
        public IEnumerable<Node> TopologicalOrderIterator();
    }
}
