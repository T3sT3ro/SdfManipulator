using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using SdfBoxController = me.tooster.sdf.Editor.Controllers.SDF.Primitives.SdfBoxController;
namespace me.tooster.sdf.Editor.Controllers.Editors.PrimitiveControllers {
    [CustomEditor(typeof(SdfBoxController), true)]
    class SdfBoxControllerEditor : SdfPrimitiveControllerEditor {
        SerializedProperty boxExtents;
        BoxBoundsHandle    boxHandle = new();

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
        public static void Instantiate() => SdfPrimitiveController.InstantiatePrimitive<SdfBoxController>("box");
    }
}
