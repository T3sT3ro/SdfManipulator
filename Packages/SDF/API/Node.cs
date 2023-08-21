#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace API {
    /// <summary>
    /// Abstraction over graph nodes as holders properties,
    /// suppliers of bodies (local generation) and properties (global generation)
    /// </summary>
    public abstract record Node(string InternalName, string DisplayName) : Representable {
        // FIXME node names should be static
        // TODO support #pragma shader_feature for node toggles
        // TODO support static (node-global) definitions and local (instance) definitions

        #region Ports

        /// Creates new input port bound to output port and this node.
        protected InputPort<T> CreateInput<T>(
            string        displayName,
            OutputPort<T> valueSource) where T : Port.Data =>
            new InputPort<T>(this, displayName, valueSource);

        /// Creates output bound to this node.
        protected OutputPort<T> CreateOutput<T>(string displayName, Func<T> evaluator)
            where T : Port.Data =>
            new OutputPort<T>(this, displayName, evaluator);

        protected InOutPort<T> CreateInOut<T>(string displayName, Func<T, T>? transform = null) where T : Port.Data =>
            new InOutPort<T>(this, displayName, transform ?? (x => x));

        #endregion

        #region introspection

        public virtual IEnumerable<T> CollectPorts<T>() where T : class, Port {
            // find all ports by reflection
            return this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(f => f.FieldType.IsSubclassOf(typeof(T)))
                .Select(f => (T)f.GetValue(this));
        }

        /// <summary>
        /// Collects all variables present on the node that should be exposed on the material as properties
        /// </summary>
        /// TODO: should variables be present on nodes other than VariableNode, considering that input ports should control node properties?
        public virtual List<Property> CollectProperties() {
            return this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.FieldType.IsSubclassOf(typeof(Property)))
                .Select(f => (Property)f.GetValue(this))
                .ToList();
        }

        // TODO: remove or replace with getter instead of attribute?
        public virtual ISet<string> CollectIncludes() {
            return this.GetType().GetCustomAttributes(typeof(ShaderIncludeAttribute), true)
                .Cast<ShaderIncludeAttribute>()
                .SelectMany(attr => attr.ShaderIncludes)
                .ToHashSet();
        }

        // TODO: remove or replace with getter instead of attribute?
        public virtual ISet<string> CollectDefines() {
            // return values of static fields annotated with ShaderDefineAttribute
            return this.GetType().GetFields(BindingFlags.Static)
                .Where(info => info.GetCustomAttributes<ShaderGlobalAttribute>().Any())
                .Select(field => (string)field.GetValue(this))
                .ToHashSet();
        }

        #endregion
    }

    /// Target node builds a final shader source for a specific target, e.g. Built-In
    // TODO: consider if a target node should have inputs and outputs that take Vertex data and Fragment data
    public abstract record TargetNode(string InternalName, string DisplayName)
        : Node(InternalName, DisplayName) {

        public abstract string BuildShaderSource();
    }
}
