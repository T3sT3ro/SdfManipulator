using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    [CustomEditor(typeof(SdfController), true)]
    public class SdfControllerEditor : ControllerEditor {
        protected virtual void  OnSceneGUI() { Gizmos.color = Handles.color = GetGizmoColor(target as SdfController); }
        protected virtual Color GetGizmoColor(SdfController controller) => controller.Inverted ? Color.red : Color.green;

        public override VisualElement CreateInspectorGUI() {
            var root = base.CreateInspectorGUI();

            return root;
        }
    }
}
