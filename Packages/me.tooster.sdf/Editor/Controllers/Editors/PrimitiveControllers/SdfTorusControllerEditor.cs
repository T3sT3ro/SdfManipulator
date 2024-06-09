using me.tooster.sdf.Editor.Controllers.SDF;
using me.tooster.sdf.Editor.Controllers.SDF.Primitives;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;
namespace me.tooster.sdf.Editor.Controllers.Editors.PrimitiveControllers {
    [CustomEditor(typeof(SdfTorusController), true)]
    class SdfTorusControllerEditor : SdfPrimitiveControllerEditor {
        SerializedProperty mainRadius;
        SerializedProperty ringRadius;
        SerializedProperty cap;

        // two ring handles for major and minor radius of torus
        SphereBoundsHandle mainRadiusHandle = new();
        SphereBoundsHandle ringRadiusHandle = new();
        // a single point cap on the main radius handle for picking a cap
        ArcHandle capArcHandle = new();

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfTorusController)target;

            var anyChanged = false;
            // approach from https://docs.unity3d.com/ScriptReference/IMGUI.Controls.ArcHandle.html

            using (new Handles.DrawingScope(Matrix4x4.TRS(controller.transform.position, controller.transform.rotation, Vector3.one))) {
                using (var mainRadiusCheck = new EditorGUI.ChangeCheckScope()) {
                    mainRadiusHandle.radius = controller.MainRadius;
                    mainRadiusHandle.axes = PrimitiveBoundsHandle.Axes.All ^ PrimitiveBoundsHandle.Axes.Y;
                    mainRadiusHandle.center = Vector3.up * controller.RingRadius;

                    mainRadiusHandle.DrawHandle();

                    anyChanged |= mainRadiusCheck.changed;
                    if (mainRadiusCheck.changed) controller.MainRadius = mainRadius.floatValue = mainRadiusHandle.radius;
                }

                using (var ringRadiusCheck = new EditorGUI.ChangeCheckScope()) {
                    ringRadiusHandle.radius = controller.RingRadius;
                    ringRadiusHandle.axes = PrimitiveBoundsHandle.Axes.All ^ PrimitiveBoundsHandle.Axes.X;
                    ringRadiusHandle.center = Vector3.forward * controller.MainRadius;

                    ringRadiusHandle.DrawHandle();

                    anyChanged |= ringRadiusCheck.changed;
                    if (ringRadiusCheck.changed) controller.RingRadius = ringRadius.floatValue = ringRadiusHandle.radius;
                }


                if (controller.Variant == SdfTorusController.TorusVariant.CAPPED) {
                    using (var capCheck = new EditorGUI.ChangeCheckScope()) {
                        capArcHandle.radius = controller.MainRadius;
                        capArcHandle.angle = controller.Cap * Mathf.Rad2Deg;

                        capArcHandle.DrawHandle();

                        anyChanged |= capCheck.changed;
                        if (capCheck.changed) controller.Cap = cap.floatValue = Mathf.Deg2Rad * capArcHandle.angle;
                    }
                }
            }

            if (anyChanged) serializedObject.ApplyModifiedProperties();
        }

        public override VisualElement CreateInspectorGUI() {
            // FIXME: don't depend on hidden internal property, maybe use source generators, expressions or expose an API 
            mainRadius = serializedObject.FindProperty(nameof(mainRadius));
            ringRadius = serializedObject.FindProperty(nameof(ringRadius));
            cap = serializedObject.FindProperty(nameof(cap));

            return base.CreateInspectorGUI();
        }


        [MenuItem("GameObject/SDF/Primitives/Torus", priority = -20)]
        public static void Instantiate() => SdfPrimitiveController.InstantiatePrimitive<SdfTorusController>("torus");
    }
}
