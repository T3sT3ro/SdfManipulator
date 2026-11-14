using me.tooster.sdf.Primitives;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace me.tooster.sdf.Editor.Inspectors.Primitives {
    [CustomEditor(typeof(SdfVolumeController), true)]
    class SdfVolumeControllerEditor : SdfPrimitiveControllerEditor {
        readonly BoxBoundsHandle boundsHandle = new();

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfVolumeController)target;

            // the SDF is evaluated in the object's space, including its scale
            using var drawingScope = new Handles.DrawingScope(controller.transform.localToWorldMatrix);
            using var check = new EditorGUI.ChangeCheckScope();
            boundsHandle.center = Vector3.zero;
            boundsHandle.size = controller.Size;

            boundsHandle.DrawHandle();

            if (!check.changed)
                return;
            Undo.RecordObject(controller, "Resize volume");
            controller.Size = boundsHandle.size;
        }

        [MenuItem("GameObject/SDF/Primitives/Volume")]
        static void Instantiate(MenuCommand command) => InstantiatePrimitive<SdfVolumeController>("volume", command);
    }
}
