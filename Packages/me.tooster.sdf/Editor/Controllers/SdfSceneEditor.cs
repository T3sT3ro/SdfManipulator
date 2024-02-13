#nullable enable
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers {
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
                sdfScene.RegenerateAssetsSafely();
            GUILayout.Space(16);

            base.OnInspectorGUI();
        }
    }
}
