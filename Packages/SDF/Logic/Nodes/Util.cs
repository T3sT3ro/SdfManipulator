#nullable enable
using System;
using System.Linq;
using API;

using Nodes.SdfNodes;
using UnityEngine;


namespace Nodes {
    public static partial class Util {

        public static SdfNode DefaultSdfNode() => new SdfSphereNode(null, null);

        // TODO: make it so that "default output port" returns a new variable node that has a non-exposed constant with default value of the type
    }
}
