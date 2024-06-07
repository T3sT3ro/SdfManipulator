using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    [CustomEditor(typeof(SdfPrimitiveController), true)]
    public class SdfPrimitiveControllerEditor : ControllerEditor {
        protected virtual void OnSceneGUI() { Gizmos.color = Handles.color = GetGizmoColor(target as Controller); }

        protected virtual Color GetGizmoColor(Controller controller)
            => controller switch
            {
                SdfController sdf => sdf.Inverted ? Color.red : Color.green,
                _                 => Color.white,
            };

        public override VisualElement CreateInspectorGUI() {
            var root = base.CreateInspectorGUI();

            return root;
        }
    }
}
