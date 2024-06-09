using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;
using SdfBoxController = me.tooster.sdf.Editor.Controllers.SDF.Primitives.SdfBoxController;
namespace me.tooster.sdf.Editor.Controllers.Editors.PrimitiveControllers {
    [CustomEditor(typeof(SdfBoxController), true)]
    class SdfBoxControllerEditor : SdfPrimitiveControllerEditor {
        SerializedProperty boxExtentsProperty;
        BoxBoundsHandle    boxHandle = new();

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfBoxController)target;
            var tr = controller.transform;

            using var check = new EditorGUI.ChangeCheckScope();

            Handles.matrix = Matrix4x4.TRS(tr.position, tr.rotation, Vector3.one);
            boxHandle.center = Vector3.zero;
            boxHandle.size = controller.BoxExtents * 2;

            boxHandle.DrawHandle();

            if (!check.changed) return;


            controller.BoxExtents = boxExtentsProperty.vector3Value = boxHandle.size / 2;
            serializedObject.ApplyModifiedProperties();
        }

        public override VisualElement CreateInspectorGUI() {
            boxExtentsProperty = serializedObject.FindProperty("boxExtents");
            return base.CreateInspectorGUI();
        }

        [MenuItem("GameObject/SDF/Primitives/Box")]
        public static void Instantiate() => SdfPrimitiveController.InstantiatePrimitive<SdfBoxController>("box");
    }
}
