using System;
using System.Collections.Generic;
using API;

namespace Builders {
    /// <summary>
    /// Abstraction over contextual shader building.
    /// It manages the state and requirements for generation.
    /// Builder captures a state of a node graph.
    /// It collects properties and others from nodes.
    /// It behaves similarly to a visitor but without a double dispatch.
    /// One instance per generation. Can be optimized in the future to cache 
    /// </summary>
    public abstract class ShaderBuilder {
        private MasterNode masterNode;

        protected readonly HashSet<string>            declaredIncludes   = new HashSet<string>();
        protected readonly Dictionary<Property, Node> declaredProperties = new Dictionary<Property, Node>();

        // Two way binds. Suppliers bind to outputs, consumers bind to inputs.
        // If all inputs are bound, the shader can be processed.
        private Dictionary<InputPort, string> nodeResults = new Dictionary<InputPort, string>();

        protected ShaderBuilder(MasterNode masterNode) { this.masterNode = masterNode; }

        public string Build() => throw new NotImplementedException();

        public string GetInputPortResult(InputPort resultPort) {
            if (nodeResults[resultPort] == null)
                throw new ShaderGenerationException($"Missing input port's result for port {resultPort}");

            return nodeResults[resultPort];
            // TODO: late-bind a result of a node connected to the input
        }

        // Definition is unique per node class (think static)
        // TODO: this can be made lazy (to support nested parameters) if definition is replaced with a supplier instead
        // public void DeclareDefinition(Node declaringNode, string definition) { }

        // Usage is unique per node instance. Should it be because ProvideResult is present already?
        // public void DeclareUsage(ShaderBuilder builder) { }

        public void DeclareIncludes(params string[] includeFiles) { declaredIncludes.UnionWith(includeFiles); }

        // TODO property headers
        public void DeclareProperties(Node declaringNode, params Property[] properties) {
            foreach (var property in properties) {
                this.declaredProperties.Add(property, declaringNode);
            }
        }
    }

    [Serializable]
    public class ShaderGenerationException : Exception {
        public ShaderGenerationException(string message) : base(message) { }
        public ShaderGenerationException(string message, Exception inner) : base(message, inner) { }
    }
}

