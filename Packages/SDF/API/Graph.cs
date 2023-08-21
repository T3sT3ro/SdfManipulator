using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace API {
    public class Graph {
        // fixme: Maybe instead of the three nodes it should just contain some way of obtaining final syntax tree? 
        public Graph(ISet<Node> nodes) { this.AllNodes = nodes; }
        
        public ISet<Node>       AllNodes    { get; private set; } // There may be nodes that are disconnected from target
        public ISet<Property>   Properties  { get; private set; }
        public ISet<TargetNode> TargetNodes => AllNodes.OfType<TargetNode>().ToHashSet();

        // this should be more trivial with the explicit bottom up graph creation model
        public static IEnumerable<Node> TopologicalOrderIterator(TargetNode targetNode) {
            var visited = new HashSet<Node> { targetNode };
            var queue = new Queue<Node>();
            queue.Enqueue(targetNode);

            while (queue.Count > 0) {
                var node = queue.Dequeue();
                yield return node;

                foreach (var port in node.CollectPorts<InputPort>()) {
                    var connectedNode = port.ConnectedOutput.Node;
                    if (visited.Contains(connectedNode))
                        continue;

                    visited.Add(connectedNode);
                    queue.Enqueue(connectedNode);
                }
            }
        }


        public Property<T> CreateProperty<T>(string displayName, T defaultValue) where T : Port.Data =>
            new Property<T>(displayName, displayName, defaultValue);

        public IReadOnlyCollection<Node> TryRemoveProperty(Property property) {
            var dependentNodes = AllNodes
                .Where(n => n.CollectProperties().Contains(property))
                .ToHashSet();

            if (dependentNodes.Count > 0)
                return dependentNodes;
            
            Properties.Remove(property);
            return Array.Empty<Node>();
        }

        public string BuildShaderForTarget(TargetNode targetNode) => targetNode.BuildShaderSource();
    }
}
