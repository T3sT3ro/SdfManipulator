using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace me.tooster.sdf.Editor.Controllers.Editors.PrimitiveControllers {
    [CustomEditor(typeof(SdfPrimitiveController), true)]
    public class SdfPrimitiveControllerEditor : ControllerEditor {
        SdfController parentSdfObject;

        void OnEnable() { parentSdfObject = (target as SdfPrimitiveController)?.GetComponent<SdfController>(); }

        protected virtual void OnSceneGUI() { Gizmos.color = Handles.color = GetGizmoColor(target as Controller); }

        protected virtual Color GetGizmoColor(Controller controller) {
            if (parentSdfObject != null) return parentSdfObject.Inverted ? Color.red : Color.green;
            return Color.white;
        }

        public override VisualElement CreateInspectorGUI() {
            var root = base.CreateInspectorGUI();

            return root;
        }
    }
}
