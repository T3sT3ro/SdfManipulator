using me.tooster.sdf.Primitives;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace me.tooster.sdf.Editor.Inspectors.Primitives {
    [CustomEditor(typeof(SdfCylinderController), true)]
    class SdfCylinderControllerEditor : SdfPrimitiveControllerEditor {
        readonly BoxBoundsHandle boxBoundsHandle = new();

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfCylinderController)target;


            // the SDF is evaluated in the object's space, including its scale
            using var drawingScope = new Handles.DrawingScope(controller.transform.localToWorldMatrix);
            using (var scope = new EditorGUI.ChangeCheckScope()) {
                boxBoundsHandle.center = Vector3.zero;
                boxBoundsHandle.size = new Vector3(controller.Radius * 2, controller.Height * 2);
                boxBoundsHandle.axes = PrimitiveBoundsHandle.Axes.All ^ PrimitiveBoundsHandle.Axes.Z;

                boxBoundsHandle.DrawHandle();

                if (scope.changed) {
                    Undo.RecordObject(controller, "Resize cylinder");
                    controller.Height = boxBoundsHandle.size.y / 2;
                    controller.Radius = boxBoundsHandle.size.x / 2;
                }
            }

            if (controller.Variant == SdfCylinderController.CylinderVariant.ROUNDED)
                DrawRoundingHandle(controller);
        }

        /// <summary>
        ///     Draws the handle for the rounding on the edge of the cylinder's outline, where the camera sees the rounding in
        ///     profile, at the end that faces the camera.
        /// </summary>
        static void DrawRoundingHandle(SdfCylinderController controller) {
            var right = CornerRadiusHandle.CameraRight();
            var radial = new Vector3(right.x, 0, right.z);
            radial = radial.sqrMagnitude > 1e-8f ? radial.normalized : Vector3.right;
            var up = Vector3.up * CornerRadiusHandle.Side(CornerRadiusHandle.TowardCamera().y);

            using var roundingCheck = new EditorGUI.ChangeCheckScope();
            var rounding = CornerRadiusHandle.Draw(
                radial * controller.Radius + up * controller.Height,
                new[] { radial, up },
                controller.Rounding,
                Mathf.Min(controller.Radius, controller.Height)
            );

            if (!roundingCheck.changed)
                return;
            Undo.RecordObject(controller, "Round cylinder");
            controller.Rounding = rounding;
        }

        [MenuItem("GameObject/SDF/Primitives/Cylinder")]
        static void Instantiate(MenuCommand command) => SdfPrimitiveControllerEditor.InstantiatePrimitive<SdfCylinderController>("cylinder", command);
    }
}
