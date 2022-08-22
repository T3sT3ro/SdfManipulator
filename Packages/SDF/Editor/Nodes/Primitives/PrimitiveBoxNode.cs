using System.Collections.Generic;
using SDF.Interface;
using SDF.Nodes.Ports;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

namespace SDF.Nodes {
    public class PrimitiveBoxNode : INode {
        public HashSet<string> Includes => new HashSet<string> { @"SDF/Includes/primitives.cginc" };

        public string     Body        => $@"{Name}";
        public string     Name        => "Box3D";
        
        
        public List<AbstractPort> InputPorts  => new List<AbstractPort>(new[] { new VectorAbstractPort()});
        public List<AbstractPort> OutputPorts => new List<AbstractPort>(new[] { new VectorAbstractPort()});
    }
    
    
}
