using me.tooster.sdf.Editor.Controllers.SDF;
using me.tooster.sdf.Editor.Controllers.SDF.Primitives;
using UnityEditor;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.Editors.PrimitiveControllers {
    [CustomEditor(typeof(SdfPlaneController), true)]
    public class SdfPlaneControllerEditor : SdfPrimitiveControllerEditor {
        protected override void OnSceneGUI() {
            base.OnSceneGUI();

            var controller = (SdfPlaneController)target;
            var tr = controller.transform;

            using (new Handles.DrawingScope(Matrix4x4.TRS(tr.position, tr.rotation, Vector3.one))) {
                // perpendicular to the disc arrow showing surface normal
                var color = Handles.color;
                color.a = 0.2f;
                Handles.color = color;
                Handles.DrawSolidDisc(Vector3.zero, Vector3.up, .5f);

                Handles.color = Color.white;
                Handles.ArrowHandleCap(0, Vector3.zero, Quaternion.LookRotation(Vector3.up), .5f, EventType.Repaint);
            }
        }

        [MenuItem("GameObject/SDF/Primitives/Plane")]
        public static void Instantiate() => SdfPrimitiveController.InstantiatePrimitive<SdfPlaneController>("plane");
    }
}
