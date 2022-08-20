using System;
using System.Collections.Generic;
using System.Linq;
using Scriban;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SDF {
    ///  A node in the graph of the SDF shader generator
    public class Node : MonoBehaviour {

        /// node template asset
        public NodeTemplate nodeTemplate;

        /// Transforms parsed templates into their final, textual representations
        public string RenderAll() {

            var rendered = new Dictionary<string, string>();
            foreach (var node in GetComponentsInChildren<Node>()) {
                if (node == this) continue;
                rendered.Add(node.name, node.RenderAll());
            }

            return nodeTemplate.Parsed.Render(rendered);
        }
    }

    
    
    #if UNITY_EDITOR    
    [CustomEditor(typeof(Node))]
    public class NodeEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            Node node = (Node)target;
            
            if (GUILayout.Button("generate")) {
                var generated = node.RenderAll();
                Debug.Log(generated);
            }
        }
    }
    #endif
}
