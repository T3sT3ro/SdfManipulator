#nullable enable
using API;
using Assets.Nodes.SdfNodes;
using PortData;

namespace Assets.Nodes {
    public abstract record SdfNode(string InternalName, string DisplayName)
        : Node(InternalName, DisplayName) {
        public abstract IOutputPort<HlslSdfFunction> sdf       { get; }
        public abstract IInputPort<HlslMatrix>       transform { get; }

        public static SdfNode Default() => new SdfSphereNode(null, null);
    }
}
