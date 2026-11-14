using me.tooster.sdf.Primitives;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace me.tooster.sdf.Editor.Inspectors.Primitives {
    [CustomEditor(typeof(SdfSphereController), true)]
    class SdfSphereControllerEditor : SdfPrimitiveControllerEditor {
        readonly SphereBoundsHandle radiusHandle = new();

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfSphereController)target;
            var tr = controller.transform;

            using (new Handles.DrawingScope(Matrix4x4.TRS(tr.position, tr.rotation, Vector3.one))) {
                using (var radiusCheck = new EditorGUI.ChangeCheckScope()) {
                    radiusHandle.center = Vector3.zero;
                    radiusHandle.radius = controller.Radius;

                    radiusHandle.DrawHandle();

                    if (!radiusCheck.changed)
                        return;

                    Undo.RecordObject(controller, "Resize sphere");
                    controller.Radius = radiusHandle.radius;
                }
            }
        }


        [MenuItem("GameObject/SDF/Primitives/Sphere")]
        static void Instantiate(MenuCommand command) => SdfPrimitiveControllerEditor.InstantiatePrimitive<SdfSphereController>("sphere", command);
    }
}
