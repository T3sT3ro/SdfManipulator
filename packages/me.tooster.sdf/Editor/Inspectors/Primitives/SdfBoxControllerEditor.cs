using me.tooster.sdf.Primitives;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using SdfBoxController = me.tooster.sdf.Primitives.SdfBoxController;
namespace me.tooster.sdf.Editor.Inspectors.Primitives {
    [CustomEditor(typeof(SdfBoxController), true)]
    class SdfBoxControllerEditor : SdfPrimitiveControllerEditor {
        readonly BoxBoundsHandle boxHandle = new();
        SerializedProperty       boxExtents;

        protected override void OnEnable() {
            base.OnEnable();
            boxExtents = serializedObject.FindProperty("boxExtents");
        }

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
            controller.BoxExtents = boxExtents.vector3Value = boxHandle.size / 2;
            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("GameObject/SDF/Primitives/Box")]
        public static void Instantiate() => SdfPrimitiveControllerEditor.InstantiatePrimitive<SdfBoxController>("box");
    }
}
