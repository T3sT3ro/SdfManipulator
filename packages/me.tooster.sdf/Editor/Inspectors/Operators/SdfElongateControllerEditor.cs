using me.tooster.sdf.Editor.Inspectors.Primitives;
using me.tooster.sdf.Operators;
using UnityEditor;
using UnityEngine;
namespace me.tooster.sdf.Editor.Inspectors.Operators {
    [CustomEditor(typeof(SdfElongateController))]
    public class SdfElongateControllerEditor : SdfPrimitiveControllerEditor {
        static readonly Color gizmoColor = new(1f, 1f, 0f, 0.4f);

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfElongateController)target;

            // Draw a dotted gizmo box of sizes
            using var drawingScope = new Handles.DrawingScope(gizmoColor, controller.transform.localToWorldMatrix);
            var l = controller.Length;
            Handles.DrawWireCube(Vector3.zero, l);
            Handles.Label(-l / 2, $"Elongate ({l.x}, {l.y}, {l.z})");
        }
    }
}
