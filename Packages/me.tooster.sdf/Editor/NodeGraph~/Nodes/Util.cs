#nullable enable

using me.tooster.sdf.Editor.NodeGraph.Nodes.SdfNodes;

namespace me.tooster.sdf.Editor.NodeGraph.Nodes {
    public static class Util {

        public static SdfNode DefaultSdfNode() => new SdfSphereNode(null, null);

        // TODO: make it so that "default output port" returns a new variable node that has a non-exposed constant with default value of the type
    }
}
