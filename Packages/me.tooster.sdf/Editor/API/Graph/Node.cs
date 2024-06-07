#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using me.tooster.sdf.Editor.Controllers.Data;
namespace me.tooster.sdf.Editor.API.Graph {
    // TODO: add script templates for new nodes: https://medium.com/miijiis-unified-works/have-unity-support-your-custom-file-part-3-6-a9820933dc84



    /// <summary>
    /// Abstraction over graph nodes as holders properties,
    /// suppliers of bodies (local generation) and properties (global generation)
    /// </summary>
    [Serializable]
    public abstract record Node(string InternalName, string DisplayName) {
        public Guid Guid { get; } = Guid.NewGuid();

        // FIXME: node names should be MOSTLY static (property node names though?)
        // TODO support #pragma shader_feature for node toggles
        // TODO support static (node-global) definitions and local (instance) definitions


        #region Ports

        /// Creates new input port bound to output port and this node.
        protected IInputPort<T> CreateInput<T>(
            string displayName,
            IOutputPort<T> valueSource
        ) where T : IData
            => new InputPort<T>(this, displayName, valueSource);

        /// Creates output bound to this node.
        protected IOutputPort<T> CreateOutput<T>(string displayName, Func<T> evaluator)
            where T : IData
            => new OutputPort<T>(this, displayName, evaluator);

        protected InOutPort<T> CreateInOut<T>(string displayName, IOutputPort<T> source, Func<T, T>? transform = null) where T : IData
            => new(this, displayName, source, transform ?? (x => x));

        #endregion


        #region introspection

        public virtual IEnumerable<T> CollectPorts<T>() where T : class, Port {
            // find all ports by reflection
            return GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(f => f.FieldType.IsSubclassOf(typeof(T)))
                .Select(f => (T)f.GetValue(this));
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
