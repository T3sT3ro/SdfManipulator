#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace me.tooster.sdf.Editor.API.Graph {
    /// <summary>
    /// A class representing a Directed Acyclic Graph of node connections. It holds a collection of properties and
    /// serves as a main interface for managing node connections and creating properties. It has several events for
    /// graph changes. Final output nodes are represented by TargetNode class. It also gives methods of traversing the
    /// graph. Data flows conceptually "from left to right" — producers on the left, consumers on the right.
    /// In the future, it could allow for runtime-safe acyclic graph creation using immutable nodes and tree rewriting
    /// using "bottom-up" (i.e. left to right) approach where consumer nodes take producer nodes as constructor params.
    /// </summary>
    [Serializable]
    public class Graph {
        public record Edge<T>(IOutputPort<T> Source, IInputPort<T> Target) where T : Data;

        [field: SerializeField] public Guid        Guid         { get; init; } = Guid.NewGuid();
        [field: SerializeField] public string      InternalName { get; }
        [field: SerializeField] public string      DisplayName  { get; }
        [field: SerializeField] public TargetNode? ActiveTarget { get; set; }

        [field: SerializeField] private ISet<Node>            allNodes   = new HashSet<Node>();
        [field: SerializeField] private ISet<Property>        properties = new HashSet<Property>();
        [field: SerializeField] private ISet<Edge<Data>> edges      = new HashSet<Edge<Data>>();

        // fixme: should properties be unique per string? should they be used by reference or by key?

        // fixme: Maybe instead of the three nodes it should just contain some way of obtaining final syntax tree? 
        public Graph(string name) { InternalName = DisplayName = name; }

        public Graph(string name, IEnumerable<Node> nodes) : this(name) {
            allNodes.UnionWith(nodes);
            var target = allNodes.OfType<TargetNode>().DefaultIfEmpty(null).First();
            if (target != null)
                ActiveTarget = target;
        }

        public IReadOnlyList<Node>             AllNodes    => allNodes.ToList();
        public IReadOnlyList<Property>         Properties  => properties.ToList();
        public IReadOnlyCollection<TargetNode> TargetNodes => AllNodes.OfType<TargetNode>().ToHashSet();

        private enum TraversalStatus : byte { Enqueued, Visited }

        // this should be a method on Graph acting as semantic model
        public static string GetUniqueNameForProperty(Property p) => throw new NotImplementedException();

        /// <summary>
        /// Iterates over nodes topologically "right to left" (reverse data direction) over collected input ports
        /// When cycle is detected, it invokes a handler for that node and skips descending into the cycle.
        /// </summary>
        /// <param name="targetNode">node to start backward traversal from</param>
        /// <exception cref="GraphException">null cycle handler throws on cycle detection</exception>
        public static IEnumerable<Node>
            NodeTopologicalIterator(Node targetNode) {
            var queue = new Queue<Node>();
            queue.Enqueue(targetNode);
            var status = new Dictionary<Node, TraversalStatus> { { targetNode, TraversalStatus.Enqueued } };

            while (queue.Count > 0) {
                var node = queue.Dequeue();
                status[node] = TraversalStatus.Visited;
                yield return node;

                foreach (var port in node.CollectPorts<IInputPort>()) {
                    var connectedNode = port.Source.Node;
                    if (!status.ContainsKey(connectedNode)) { // fresh node discovered
                        queue.Enqueue(connectedNode);
                        status[connectedNode] = TraversalStatus.Enqueued;
                    } else if (status[connectedNode] == TraversalStatus.Visited) { // cycle detected
                        // here would be a cycleHandler
                        throw new GraphException(string.Format(
                            "Cycle detected in graph between node {0} and {1}",
                            node, connectedNode)
                        );
                    } // else skip adding an already enqueued node
                }
            }
        }

        /// DFS search over input ports to check if edge would create a cycle 
        private static bool EdgeFormsCycle<T>(IOutputPort<T> source, IInputPort<T> target) where T : Data {
            foreach (var node in NodeTopologicalIterator(source.Node)) {
                if (node == target.Node)
                    return true;
            }

            return false;
        }

        public string BuildActiveTarget() => ActiveTarget == null ? "" : BuildShaderForTarget(ActiveTarget);
        public string BuildShaderForTarget(TargetNode targetNode) => targetNode.BuildShaderSource();

        // TODO use references and out variables for example TryAddNode<T>(out var newNode). For that a default parameterless ctor should be present in node
        public bool TryAddNode(Node node) {
            if (AllNodes.Contains(node))
                return false;

            allNodes.Add(node);
            OnNodeAdded(node);
            return true;
        }

        public bool TryRemoveNode(Node node) {
            if (!AllNodes.Contains(node))
                return false;

            allNodes.Remove(node);
            OnNodeRemoved(node);
            return true;
        }

        
        // using reflection
        public static IEnumerable<Node> CollectNodes(Object o) =>
            o.GetType().GetFields(BindingFlags.Instance)
                .Select(field => field.GetValue(o)).OfType<Node>();
        
        /// <summary>
        /// Attempts to connect ports. Disconnects previous connection. Invokes OnConnect event after connection.
        /// Doesn't invoke OnDisconnect on the disconnected node.
        /// </summary>
        /// <returns>true if connection was successful, false otherwise</returns>
        public bool TryConnect<T>(IOutputPort<T> source, IInputPort<T> target) where T : Data {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            // cases:
            // 1. ports are already correctly connected
            // 2. target port stays fixed, source port changes (remove previous connection to target) 
            // 2*. source port stays fixed, target port changes (add new source connection, remove previous target connection)
            // 3. a connection would form a cycle
            // The 2*. case is essentially 2. because multiple output connections are allowed and all nodes have 
            // default inputs, meaning that previous input would be disconnected

            if (target.Source == source) // 1.
                return false;

            // 3. I proved, that there is no need to remove previous connection. New connection can't form a
            // loop using edge-to-be-removed if graph was a DAG before. A prove by drawing possible 3 node dags
            if (EdgeFormsCycle(source, target))
                return false;

            // 2.
            target.Source.UnsafeRemoveTarget(target);
            target.UnsafeSetSource(source);
            source.UnsafeAddTarget(target);

            OnConnected(target, source);
            return true;
        }

        /*
        /// <summary>
        /// If any node depends on this property, it is not removed and a collection of dependent nodes is returned.
        /// Otherwise an empty collection is returned and the property is removed from the graph.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public IReadOnlyCollection<Node> TryRemoveProperty(Property property) {
            var dependentNodes = AllNodes
                .Where(n => n.CollectProperties().Contains(property))
                .ToHashSet();

            if (dependentNodes.Count > 0)
                return dependentNodes;

            properties.Remove(property);
            return Array.Empty<Node>();
        }
        */


        #region events and delegates

        public delegate void NodeEvent(Node node);

        public delegate void ConnectionEvent(IInputPort input, IOutputPort output);

        public delegate void PropertyEvent(Property property);

        public event NodeEvent       OnNodeAdded       = delegate { };
        public event NodeEvent       OnNodeRemoved     = delegate { };
        public event ConnectionEvent OnConnected       = delegate { };
        public event ConnectionEvent OnDisconnected    = delegate { };
        public event PropertyEvent   OnPropertyCreated = delegate { };
        public event PropertyEvent   OnPropertyDeleted = delegate { };

        #endregion

        public class GraphException : Exception {
            public GraphException(string message) : base(message) { }
        }
    }
}
