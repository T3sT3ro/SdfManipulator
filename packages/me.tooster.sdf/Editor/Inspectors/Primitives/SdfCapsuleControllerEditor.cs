using me.tooster.sdf.Primitives;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace me.tooster.sdf.Editor.Inspectors.Primitives {
    [CustomEditor(typeof(SdfCapsuleController), true)]
    class SdfCapsuleControllerEditor : SdfPrimitiveControllerEditor {
        readonly CapsuleBoundsHandle capsuleHandle = new() { heightAxis = CapsuleBoundsHandle.HeightAxis.Y };

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfCapsuleController)target;

            // the SDF is evaluated in the object's space, including its scale
            using var drawingScope = new Handles.DrawingScope(controller.transform.localToWorldMatrix);
            using var check = new EditorGUI.ChangeCheckScope();
            capsuleHandle.center = Vector3.zero;
            capsuleHandle.radius = controller.Radius;
            // the handle's height covers the rounded ends
            capsuleHandle.height = 2 * (controller.Height + controller.Radius);

            capsuleHandle.DrawHandle();

            if (!check.changed)
                return;
            Undo.RecordObject(controller, "Resize capsule");
            controller.Radius = capsuleHandle.radius;
            controller.Height = Mathf.Max(0, capsuleHandle.height / 2 - capsuleHandle.radius);
        }

        [MenuItem("GameObject/SDF/Primitives/Capsule")]
        static void Instantiate(MenuCommand command) => InstantiatePrimitive<SdfCapsuleController>("capsule", command);
    }
}
