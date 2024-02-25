using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    [CustomEditor(typeof(TransformController))]
    public class TransformControllerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var controller = (TransformController)target;
            Handles.color = Color.red;
            Handles.DrawWireCube(Vector3.zero, controller.transform.localScale);
        }
    }
}
