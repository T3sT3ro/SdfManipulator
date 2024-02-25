#nullable enable
using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    [CustomEditor(typeof(SdfScene))]
    public class SdfSceneEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            var sdfScene = (SdfScene)target;

            if (!sdfScene.controlledShader) {
                EditorGUILayout.HelpBox("missing controlled shader asset", MessageType.Error);
                base.OnInspectorGUI();
                return;
            }

            if (!sdfScene.raymarchingShader) {
                EditorGUILayout.HelpBox("Raymarching shader asset required", MessageType.Error);
                base.OnInspectorGUI();
                return;
            }

            if (GUILayout.Button("Rebuild shader"))
                sdfScene.RegisterForRegeneration();
            if (GUILayout.Button("Open generated shader"))
                AssetDatabase.OpenAsset(sdfScene.controlledShader);
            if (GUILayout.Button("Assign material to renderer")) {
                var renderer = sdfScene.GetComponent<Renderer>();
                if (renderer) renderer.material = sdfScene.controlledMaterial;
                else EditorGUILayout.HelpBox("No renderer found", MessageType.Error);
            }
            GUILayout.Space(16);
            base.OnInspectorGUI();
        }
    }
}
