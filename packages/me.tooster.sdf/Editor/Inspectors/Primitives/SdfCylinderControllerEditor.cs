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
            using var scope = new EditorGUI.ChangeCheckScope();

            boxBoundsHandle.center = Vector3.zero;
            boxBoundsHandle.size = new Vector3(controller.Radius * 2, controller.Height * 2);
            boxBoundsHandle.axes = PrimitiveBoundsHandle.Axes.All ^ PrimitiveBoundsHandle.Axes.Z;

            boxBoundsHandle.DrawHandle();

            if (!scope.changed)
                return;

            Undo.RecordObject(controller, "Resize cylinder");
            controller.Height = boxBoundsHandle.size.y / 2;
            controller.Radius = boxBoundsHandle.size.x / 2;
        }

        [MenuItem("GameObject/SDF/Primitives/Cylinder")]
        static void Instantiate(MenuCommand command) => SdfPrimitiveControllerEditor.InstantiatePrimitive<SdfCylinderController>("cylinder", command);
    }
}
