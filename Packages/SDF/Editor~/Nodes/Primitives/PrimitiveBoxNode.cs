using System.Collections.Generic;
using Editor.Nodes;
using SDF.Interface;
using SDF.Nodes.Ports;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;
using Input = UnityEngine.Windows.Input;

namespace SDF.Nodes {
    public class PrimitiveBoxNode : INode, ISdfNode {
        public HashSet<string> Includes => new HashSet<string> { @"Packages/SDF/Editor/Includes/primitives.cginc" };

        public string     Body        => $@"{Name}";
        public string     Name        => "Box3D";

        public INode  SizeInput;

        public Template Sdf    => Template.Parse(@"sdf::primitives::box3d({{p}}, {{dim}})");
        public Object   PartialModel => new Object();
    }
}
