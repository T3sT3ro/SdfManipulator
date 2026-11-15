using me.tooster.sdf.Primitives;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using SdfBoxController = me.tooster.sdf.Primitives.SdfBoxController;
namespace me.tooster.sdf.Editor.Inspectors.Primitives {
    [CustomEditor(typeof(SdfBoxController), true)]
    class SdfBoxControllerEditor : SdfPrimitiveControllerEditor {
        readonly BoxBoundsHandle boxHandle = new();

        protected override void OnSceneGUI() {
            base.OnSceneGUI();

            var controller = (SdfBoxController)target;
            var tr = controller.transform;

            using var drawingScope = new Handles.DrawingScope(tr.localToWorldMatrix);
            using (var extentsCheck = new EditorGUI.ChangeCheckScope()) {
                boxHandle.center = Vector3.zero;
                boxHandle.size = controller.BoxExtents * 2;

                boxHandle.DrawHandle();

                if (extentsCheck.changed) {
                    Undo.RecordObject(controller, "Resize box");
                    controller.BoxExtents = boxHandle.size / 2;
                }
            }

            // on the corner that faces the camera
            using (var roundingCheck = new EditorGUI.ChangeCheckScope()) {
                var extents = controller.BoxExtents;
                var side = CornerRadiusHandle.TowardCamera();
                var signs = new Vector3(CornerRadiusHandle.Side(side.x), CornerRadiusHandle.Side(side.y), CornerRadiusHandle.Side(side.z));
                var rounding = CornerRadiusHandle.Draw(
                    Vector3.Scale(signs, extents),
                    new[] { Vector3.right * signs.x, Vector3.up * signs.y, Vector3.forward * signs.z },
                    controller.Rounding,
                    Mathf.Min(extents.x, Mathf.Min(extents.y, extents.z))
                );

                if (roundingCheck.changed) {
                    Undo.RecordObject(controller, "Round box");
                    controller.Rounding = rounding;
                }
            }
        }

        [MenuItem("GameObject/SDF/Primitives/Box")]
        static void Instantiate(MenuCommand command) => SdfPrimitiveControllerEditor.InstantiatePrimitive<SdfBoxController>("box", command);
    }
}
