using System;
using System.Collections.Generic;
using API;
using UnityEditor;

namespace Builders {
    /// <summary>
    /// Abstraction over language agnostic contextual shader building.
    /// It manages the state and requirements for generation.
    /// Builder captures a state of a node graph.
    /// It collects properties and others from nodes.
    /// One instance per generation. Can be optimized in the future to cache
    /// </summary>
    public class ShaderBuilder : Builder<string, MasterNode> {
        private Dictionary<Type, Type> registeredBuilders; // typeof(Node) to typeof(NodeBuilder)
        // Two way binds. Suppliers bind to outputs, consumers bind to inputs.
        // If all inputs are bound, the shader can be processed.
        private Dictionary<OutputPort, string> nodeResults = new Dictionary<OutputPort, string>();
        
        public   HashSet<string>            DeclaredIncludes   { get; } = new HashSet<string>();
        public   Dictionary<Property, Node> DeclaredProperties { get; } = new Dictionary<Property, Node>();


        public ShaderBuilder(Dictionary<Type, Type> registeredBuilders) {
            this.registeredBuilders = registeredBuilders;
        }

        public string GetResult(InputPort inputPort) {
            var connectedOutputPort = inputPort.ConnectedPort;
            var resultFound = nodeResults.TryGetValue(connectedOutputPort, out var result);
            if (!resultFound)
                throw new ShaderGenerationException($"Missing input port's result for port {inputPort}");
            
            return result;
            // TODO: late-bind a result of a node connected to the input
        }

        // Definition is unique per node class (think static)
        // TODO: this can be made lazy (to support nested parameters) if definition is replaced with a supplier instead
        // public void DeclareDefinition(Node declaringNode, string definition) { }

        // Usage is unique per node instance. Should it be because ProvideResult is present already?
        // public void DeclareUsage(ShaderBuilder builder) { }

        public void DeclareIncludes(params string[] includeFiles) { DeclaredIncludes.UnionWith(includeFiles); }

        private void BuildNode(Node node) {
            if (!registeredBuilders.TryGetValue(node.GetType(), out var builderType))
                throw new ShaderGenerationException($"missing generator for {node.GetType()}");
            
            var builder = (NodeBuilder<Node>)Activator.CreateInstance(builderType, this);
            foreach (var property in builder.DeclaredProperties)
                this.DeclaredProperties.Add(property, node);
        }

        public string Build(MasterNode masterNode) {
            // TODO traverse in topological order
            throw new NotImplementedException("DAG traversal WIP");
        }

        public string GetUniqueIdentifierForNode(Node node) => $"{node.InternalName}_{GUID.Generate().ToString()}";
    }

    [Serializable]
    public class ShaderGenerationException : Exception {
        public ShaderGenerationException(string message) : base(message) { }
        public ShaderGenerationException(string message, Exception inner) : base(message, inner) { }
    }
}
