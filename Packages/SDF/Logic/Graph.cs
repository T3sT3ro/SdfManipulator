using System.Collections.Generic;
using API;

public class Graph : API.Graph {
    public VertInNode        VertIn                     { get; private set; }
    public VertToFragNode    VertToFrag                 { get; private set; }
    public FragOutNode       FragOut                    { get; private set; }

    public Graph(VertInNode vertIn, VertToFragNode vertToFrag, FragOutNode fragOut) {
        VertIn = vertIn;
        VertToFrag = vertToFrag;
        FragOut = fragOut;
    }
    
    public IEnumerable<Node> TopologicalOrderIterator() {
        var rootNodes = new List<Node> { VertIn, VertToFrag, FragOut };
        var visited = new HashSet<Node>(rootNodes);
        var queue = new Queue<Node>(rootNodes);

        while (queue.Count > 0) {
            var node = queue.Dequeue();
            yield return node;

            foreach (var port in node.CollectPorts<InputPort>()) {
                if (port.IncomingConnection == null)
                    continue;

                var connectedNode = port.IncomingConnection.Node;
                if (visited.Contains(connectedNode))
                    continue;

                visited.Add(connectedNode);
                queue.Enqueue(connectedNode);
            }
        }
    }
}
