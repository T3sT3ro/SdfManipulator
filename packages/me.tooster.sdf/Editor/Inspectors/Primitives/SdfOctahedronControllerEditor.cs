using me.tooster.sdf.Primitives;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace me.tooster.sdf.Editor.Inspectors.Primitives {
    [CustomEditor(typeof(SdfOctahedronController), true)]
    class SdfOctahedronControllerEditor : SdfPrimitiveControllerEditor {
        // the vertices lie on a sphere of radius Size
        readonly SphereBoundsHandle sizeHandle = new();

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfOctahedronController)target;

            // the SDF is evaluated in the object's space, including its scale
            using var drawingScope = new Handles.DrawingScope(controller.transform.localToWorldMatrix);
            using var check = new EditorGUI.ChangeCheckScope();
            sizeHandle.center = Vector3.zero;
            sizeHandle.radius = controller.Size;

            sizeHandle.DrawHandle();

            if (!check.changed)
                return;
            Undo.RecordObject(controller, "Resize octahedron");
            controller.Size = sizeHandle.radius;
        }

        [MenuItem("GameObject/SDF/Primitives/Octahedron")]
        static void Instantiate(MenuCommand command) => InstantiatePrimitive<SdfOctahedronController>("octahedron", command);
    }
}
