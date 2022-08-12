using Scriban;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SDF {
    
    [CreateAssetMenu(order = 0, fileName = "SdfScene", menuName = "SDF/Scene")]
    public class Scene : ScriptableObject {
        private Node   root;
        public  string output;
        
        public Shader   shader;
        public Material material;
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(Scene))]
    public class SceneEditor : Editor {
        
        
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Update Shader")) {
                
            }
        }
    }
    
    // [ExcludeFromPreset]
    // [ScriptedImporter(127, Extension)]
    // public class SdfGraphImporter : ScriptedImporter {
    //     public const    string Extension = "sdfeditor";
    //     public override void   OnImportAsset(AssetImportContext ctx) { throw new System.NotImplementedException(); }
    // }
    #endif
}
