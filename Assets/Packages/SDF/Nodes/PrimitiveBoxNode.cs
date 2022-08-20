using System.Collections.Generic;
using SDF.Interface;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

namespace SDF.Nodes {
    public class PrimitiveBoxNode : ISdfNode {
        public string                       Include    => @"SDF/Includes/primitives.cginc";
        public string                       Body       => $@"{Name}";
        public string                       Name       => "Box3D";
        public List<AbstractShaderProperty> Properties { get; }
    }
}
