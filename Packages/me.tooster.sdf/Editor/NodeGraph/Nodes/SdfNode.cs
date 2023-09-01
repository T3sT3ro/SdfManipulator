#nullable enable
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.Nodes.SdfNodes;
using me.tooster.sdf.Editor.NodeGraph.PortData;

namespace me.tooster.sdf.Editor.NodeGraph.Nodes {
    public abstract record SdfNode(string InternalName, string DisplayName)
        : Node(InternalName, DisplayName) {
        public abstract IOutputPort<HlslSdfFunction> sdf       { get; }
        public abstract IInputPort<HlslMatrix>       transform { get; }

        public static SdfNode Default() => new SdfSphereNode(null, null);
    }
}
