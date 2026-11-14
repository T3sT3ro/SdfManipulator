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

            using var drawingScope = new Handles.DrawingScope(Matrix4x4.TRS(tr.position, tr.rotation, Vector3.one));
            using var extentsCheck = new EditorGUI.ChangeCheckScope();
            boxHandle.center = Vector3.zero;
            boxHandle.size = controller.BoxExtents * 2;

            boxHandle.DrawHandle();

            if (!extentsCheck.changed)
                return;
            Undo.RecordObject(controller, "Resize box");
            controller.BoxExtents = boxHandle.size / 2;
        }

        [MenuItem("GameObject/SDF/Primitives/Box")]
        static void Instantiate(MenuCommand command) => SdfPrimitiveControllerEditor.InstantiatePrimitive<SdfBoxController>("box", command);
    }
}
