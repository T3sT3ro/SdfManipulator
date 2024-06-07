using me.tooster.sdf.Editor.Controllers.SDF;
using me.tooster.sdf.Editor.Controllers.SDF.Primitives;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    [CustomEditor(typeof(SdfConeController), true)]
    class SdfConeControllerEditor : SdfPrimitiveControllerEditor {
        SerializedProperty angle;
        SerializedProperty height;

        SphereBoundsHandle radiusHandle = new();

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfConeController)target;


            using var scope = new EditorGUI.ChangeCheckScope();

            radiusHandle.radius = controller.Height * Mathf.Tan(controller.Angle);
            radiusHandle.axes = PrimitiveBoundsHandle.Axes.All ^ PrimitiveBoundsHandle.Axes.Y;
            radiusHandle.center = controller.ConeOrigin == SdfConeController.OriginPosition.TIP
                ? Vector3.up * -controller.Height
                : Vector3.zero;

            Handles.matrix = Matrix4x4.TRS(controller.transform.position, controller.transform.rotation, Vector3.one);

            radiusHandle.DrawHandle();

            // handle for changing the "height" as a simple line and a dot either to the cone tip or the base, depending which origin mode is set
            var heightHandlePos = controller.ConeOrigin == SdfConeController.OriginPosition.TIP
                ? Vector3.up * -controller.Height
                : Vector3.up * controller.Height;

            var tipPosition = Handles.Slider(
                heightHandlePos,
                Vector3.up,
                HandleUtility.GetHandleSize(heightHandlePos) * .2f,
                Handles.CircleHandleCap,
                -1
            );
            Handles.DrawLine(radiusHandle.center, tipPosition);
            Handles.Label(tipPosition / 2, $"Height: {tipPosition.y}");

            if (!scope.changed) return;

            controller.Angle = angle.floatValue = Mathf.Atan2(radiusHandle.radius, tipPosition.y);
            controller.Height = height.floatValue = tipPosition.y;

            serializedObject.ApplyModifiedProperties();
        }

        public override VisualElement CreateInspectorGUI() {
            // FIXME: don't depend on hidden internal property, maybe use source generators, expressions or expose an API 
            angle = serializedObject.FindProperty(nameof(angle));
            height = serializedObject.FindProperty(nameof(height));
            return base.CreateInspectorGUI();
        }


        [MenuItem("GameObject/SDF/Primitives/Cone", priority = -20)]
        public static void Instantiate() => Controller.TryInstantiate<SdfConeController>("cone");
    }
}
