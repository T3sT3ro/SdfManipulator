using me.tooster.sdf.Editor.Controllers.SDF;
using me.tooster.sdf.Editor.Controllers.SDF.Primitives;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.Editors.PrimitiveControllers {
    [CustomEditor(typeof(SdfSphereController), true)]
    class SdfSphereControllerEditor : SdfPrimitiveControllerEditor {
        SerializedProperty radiusProperty;

        SphereBoundsHandle radiusHandle = new();

        protected override void OnEnable() {
            base.OnEnable();
            // FIXME: don't depend on hidden internal property, maybe use source generators, expressions or expose an API 
            radiusProperty = serializedObject.FindProperty("radius");
        }

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfSphereController)target;
            var tr = controller.transform;

            using (new Handles.DrawingScope(Matrix4x4.TRS(tr.position, tr.rotation, Vector3.one))) {
                using (var radiusCheck = new EditorGUI.ChangeCheckScope()) {
                    radiusHandle.center = Vector3.zero;
                    radiusHandle.radius = controller.Radius;

                    radiusHandle.DrawHandle();

                    if (!radiusCheck.changed) return;

                    controller.Radius = radiusProperty.floatValue = radiusHandle.radius;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }


        [MenuItem("GameObject/SDF/Primitives/Sphere", priority = -20)]
        public static void Instantiate() => SdfPrimitiveController.InstantiatePrimitive<SdfSphereController>("sphere");
    }
}
