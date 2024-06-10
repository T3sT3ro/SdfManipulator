using me.tooster.sdf.Editor.Controllers.SDF;
using me.tooster.sdf.Editor.Controllers.SDF.Primitives;
using UnityEditor;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.Editors.PrimitiveControllers {
    [CustomEditor(typeof(SdfSphereController), true)]
    class SdfSphereControllerEditor : SdfPrimitiveControllerEditor {
        SerializedProperty radiusProperty;

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
                    var radius = Handles.RadiusHandle(Quaternion.identity, controller.transform.position, controller.Radius);

                    if (!radiusCheck.changed) return;

                    controller.Radius = radiusProperty.floatValue = radius;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }


        [MenuItem("GameObject/SDF/Primitives/Sphere", priority = -20)]
        public static void Instantiate() => SdfPrimitiveController.InstantiatePrimitive<SdfSphereController>("sphere");
    }
}
