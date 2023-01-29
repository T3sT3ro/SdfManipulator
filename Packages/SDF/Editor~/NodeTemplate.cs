using Scriban;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SDF {
    /// this asset holds the HLSL template for nodes of this type
    [CreateAssetMenu(order = 0, fileName = "NewNodeTemplate", menuName = "SDF/NodeTemplate")]
    public class NodeTemplate : ScriptableObject {

        
        /// template part for HLSL
        [TextArea(20, 40)] public string hlslTemplate;

        public Template Parsed  { get; private set; }

        public string[] includes;
        public string[] requires;
        public string[] provides;

        private void OnValidate() {
            Parsed = Template.Parse(hlslTemplate);
        }
    }
    
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(NodeTemplate))]
    public class NodeTemplateEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            NodeTemplate template = (NodeTemplate)target;
            base.OnInspectorGUI();
            
            if (template.Parsed.HasErrors) {
                foreach (var msg in template.Parsed.Messages) {
                    Debug.LogError(msg.Message);
                    EditorGUILayout.HelpBox(msg.Message, MessageType.Error);
                }
            }
        }
    }
#endif
}
