using System.Collections.Generic;
using Scriban;
using UnityEditor.AssetImporters;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace SDF {
    [CreateAssetMenu(order = 0, fileName = "SdfNode", menuName = "SDF/TestNode")]
    public class NodeTemplate : ScriptableObject {
        [TextArea]
        public string                     template;
    }

    public interface Node {
        public string Template { get; } 
    }

    public class RootNode : Node {
        public string Template => "test {{test}}";
    }

    [CreateAssetMenu(order = 0, fileName = "SdfScene", menuName = "SDF/Scene")]
    public class Scene : ScriptableObject {
        private Node   _root;
        public  string output;
        
        public void generateMaterial() {
            var template = Template.Parse(_root.Template);
            output = template.Render(new {test = "something"});
        }
        
        
    }
    
    [ExcludeFromPreset]
    [ScriptedImporter(127, Extension)]
    public class SdfGraphImporter : ScriptedImporter {
        public const    string Extension = "sdfeditor";
        public override void   OnImportAsset(AssetImportContext ctx) { throw new System.NotImplementedException(); }
    }
}
