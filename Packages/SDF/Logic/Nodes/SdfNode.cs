#nullable enable
using API;
using Nodes.SdfNodes;
using PortData;
using UnityEngine;

namespace Nodes {
    public abstract record SdfNode(string InternalName, string DisplayName)
        : Node(InternalName, DisplayName) {
        public OutputPort<HlslSdfFunction> sdf { get; protected init; }
        
        public static SdfNode Default() => new SdfSphereNode(null, null);
    }
}
