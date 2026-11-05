using me.tooster.sdf.Editor.Controllers.Editors.PrimitiveControllers;
using me.tooster.sdf.Editor.Controllers.SDF.Operators;
using UnityEditor;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.Editors.Operators {
    [CustomEditor(typeof(SdfElongateController))]
    public class SdfElongateControllerEditor : SdfPrimitiveControllerEditor {
        static readonly Color gizmoColor = new(1f, 1f, 0f, 0.4f);

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfElongateController)target;

            // Draw a dotted gizmo box of sizes
            Handles.matrix = Matrix4x4.TRS(controller.transform.position, controller.transform.rotation, Vector3.one);
            Handles.color = gizmoColor;
            var l = controller.Length;
            Handles.DrawWireCube(Vector3.zero, l);
            Handles.Label(-l / 2, $"Elongate ({l.x}, {l.y}, {l.z})");
        }
    }
}
