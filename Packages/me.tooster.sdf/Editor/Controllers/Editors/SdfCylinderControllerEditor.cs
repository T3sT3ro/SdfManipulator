using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    [CustomEditor(typeof(SdfCylinderController), true)]
    class SdfCylinderControllerEditor : SdfControllerEditor {
        SerializedProperty heightProperty;
        SerializedProperty radiusProperty;
        SerializedProperty roundingProperty;

        readonly BoxBoundsHandle boxBoundsHandle = new();

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfCylinderController)target;


            using var scope = new EditorGUI.ChangeCheckScope();

            boxBoundsHandle.center = Vector3.zero;
            boxBoundsHandle.size = new Vector3(controller.Radius * 2, controller.Height * 2);
            boxBoundsHandle.axes = PrimitiveBoundsHandle.Axes.All ^ PrimitiveBoundsHandle.Axes.Z;

            Handles.matrix = Matrix4x4.TRS(controller.transform.position, controller.transform.rotation, Vector3.one);

            boxBoundsHandle.DrawHandle();

            if (!scope.changed) return;

            controller.Height = heightProperty.floatValue = boxBoundsHandle.size.y / 2;
            controller.Radius = radiusProperty.floatValue = boxBoundsHandle.size.x / 2;


            serializedObject.ApplyModifiedProperties();
        }

        public override VisualElement CreateInspectorGUI() {
            // FIXME: don't depend on hidden internal property, maybe use source generators, expressions or expose an API 
            heightProperty = serializedObject.FindProperty("height");
            radiusProperty = serializedObject.FindProperty("radius");
            roundingProperty = serializedObject.FindProperty("rounding");
            return base.CreateInspectorGUI();
        }


        [MenuItem("GameObject/SDF/Primitives/Cylinder", priority = -20)]
        public static void Instantiate() => Controller.TryInstantiate<SdfCylinderController>("cylinder");
    }
}
