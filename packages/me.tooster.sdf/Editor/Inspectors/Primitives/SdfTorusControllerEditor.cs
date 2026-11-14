using me.tooster.sdf.Primitives;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace me.tooster.sdf.Editor.Inspectors.Primitives {
    [CustomEditor(typeof(SdfTorusController), true)]
    class SdfTorusControllerEditor : SdfPrimitiveControllerEditor {
        // a single point cap on the main radius handle for picking a cap
        readonly ArcHandle capArcHandle = new();

        // two ring handles for major and minor radius of torus
        readonly SphereBoundsHandle mainRadiusHandle = new();
        readonly SphereBoundsHandle ringRadiusHandle = new();

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfTorusController)target;
            // approach from https://docs.unity3d.com/ScriptReference/IMGUI.Controls.ArcHandle.html

            using (new Handles.DrawingScope(Matrix4x4.TRS(controller.transform.position, controller.transform.rotation, Vector3.one))) {
                using (var mainRadiusCheck = new EditorGUI.ChangeCheckScope()) {
                    mainRadiusHandle.radius = controller.MainRadius;
                    mainRadiusHandle.axes = PrimitiveBoundsHandle.Axes.All ^ PrimitiveBoundsHandle.Axes.Y;
                    mainRadiusHandle.center = Vector3.zero; // TODO: remove alltogether and the handle will be pinned to the opposite side
                    // TODO, depending on player prefs, use the this to display on the surface: Vector3.up * controller.RingRadius;

                    mainRadiusHandle.DrawHandle();

                    if (mainRadiusCheck.changed) {
                        Undo.RecordObject(controller, "Resize torus");
                        controller.MainRadius = mainRadiusHandle.radius;
                    }
                }

                using (var ringRadiusCheck = new EditorGUI.ChangeCheckScope()) {
                    ringRadiusHandle.radius = controller.RingRadius;
                    ringRadiusHandle.axes = PrimitiveBoundsHandle.Axes.All ^ PrimitiveBoundsHandle.Axes.X;
                    ringRadiusHandle.center = Vector3.forward * controller.MainRadius;

                    ringRadiusHandle.DrawHandle();

                    if (ringRadiusCheck.changed) {
                        Undo.RecordObject(controller, "Resize torus");
                        controller.RingRadius = ringRadiusHandle.radius;
                    }
                }


                if (controller.Variant == SdfTorusController.TorusVariant.CAPPED) {
                    using (var capCheck = new EditorGUI.ChangeCheckScope()) {
                        capArcHandle.radius = controller.MainRadius;
                        capArcHandle.angle = controller.Cap * Mathf.Rad2Deg;

                        capArcHandle.DrawHandle();

                        if (capCheck.changed) {
                            Undo.RecordObject(controller, "Change torus cap");
                            controller.Cap = Mathf.Deg2Rad * capArcHandle.angle;
                        }
                    }
                }
            }
        }


        [MenuItem("GameObject/SDF/Primitives/Torus")]
        static void Instantiate(MenuCommand command) => SdfPrimitiveControllerEditor.InstantiatePrimitive<SdfTorusController>("torus", command);
    }
}
