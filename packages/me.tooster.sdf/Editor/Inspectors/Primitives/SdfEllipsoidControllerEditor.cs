using me.tooster.sdf.Primitives;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace me.tooster.sdf.Editor.Inspectors.Primitives {
    [CustomEditor(typeof(SdfEllipsoidController), true)]
    class SdfEllipsoidControllerEditor : SdfPrimitiveControllerEditor {
        readonly BoxBoundsHandle boundsHandle = new();

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfEllipsoidController)target;

            // the SDF is evaluated in the object's space, including its scale
            using var drawingScope = new Handles.DrawingScope(controller.transform.localToWorldMatrix);
            using var check = new EditorGUI.ChangeCheckScope();
            boundsHandle.center = Vector3.zero;
            boundsHandle.size = controller.Radii * 2;

            boundsHandle.DrawHandle();

            if (!check.changed)
                return;
            Undo.RecordObject(controller, "Resize ellipsoid");
            controller.Radii = boundsHandle.size / 2;
        }

        [MenuItem("GameObject/SDF/Primitives/Ellipsoid")]
        static void Instantiate(MenuCommand command) => InstantiatePrimitive<SdfEllipsoidController>("ellipsoid", command);
    }
}
