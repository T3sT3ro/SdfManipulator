using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    [CustomEditor(typeof(SdfSphereController), true)]
    class SdfSphereControllerEditor : SdfControllerEditor {
        SerializedProperty radiusProperty;

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfSphereController)target;
            using var scope = new EditorGUI.ChangeCheckScope();
            var radius = Handles.RadiusHandle(Quaternion.identity, controller.transform.position, controller.Radius);
            if (!scope.changed) return;

            controller.Radius = radiusProperty.floatValue = radius;
            serializedObject.ApplyModifiedProperties();
        }

        public override VisualElement CreateInspectorGUI() {
            // FIXME: don't depend on hidden internal property, maybe use source generators, expressions or expose an API 
            radiusProperty = serializedObject.FindProperty("radius");
            return base.CreateInspectorGUI();
        }


        [MenuItem("GameObject/SDF/Primitives/Sphere", priority = -20)]
        public static void Instantiate() => Controller.TryInstantiate<SdfSphereController>("sphere");
    }
}
