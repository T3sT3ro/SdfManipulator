using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    [CustomEditor(typeof(SdfTorusController), true)]
    class SdfTorusControllerEditor : SdfControllerEditor {
        SerializedProperty mainRadius;
        SerializedProperty ringRadius;

        SphereBoundsHandle mainRadiusHandle = new();
        SphereBoundsHandle ringRadiusHandle = new();

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfTorusController)target;


            using var scope = new EditorGUI.ChangeCheckScope();

            Handles.matrix = Matrix4x4.TRS(controller.transform.position, controller.transform.rotation, Vector3.one);

            mainRadiusHandle.radius = controller.MainRadius;
            mainRadiusHandle.axes = PrimitiveBoundsHandle.Axes.All ^ PrimitiveBoundsHandle.Axes.Y;
            mainRadiusHandle.center = Vector3.zero;

            mainRadiusHandle.DrawHandle();

            ringRadiusHandle.radius = controller.RingRadius;
            ringRadiusHandle.axes = PrimitiveBoundsHandle.Axes.All ^ PrimitiveBoundsHandle.Axes.Z;
            ringRadiusHandle.center = Vector3.right * controller.MainRadius;

            ringRadiusHandle.DrawHandle();

            if (!scope.changed) return;

            controller.MainRadius = mainRadius.floatValue = mainRadiusHandle.radius;
            controller.RingRadius = ringRadius.floatValue = ringRadiusHandle.radius;

            serializedObject.ApplyModifiedProperties();
        }

        public override VisualElement CreateInspectorGUI() {
            // FIXME: don't depend on hidden internal property, maybe use source generators, expressions or expose an API 
            mainRadius = serializedObject.FindProperty(nameof(mainRadius));
            ringRadius = serializedObject.FindProperty(nameof(ringRadius));
            return base.CreateInspectorGUI();
        }


        [MenuItem("GameObject/SDF/Primitives/Torus", priority = -20)]
        public static void Instantiate() => Controller.TryInstantiate<SdfTorusController>("torus");
    }
}
