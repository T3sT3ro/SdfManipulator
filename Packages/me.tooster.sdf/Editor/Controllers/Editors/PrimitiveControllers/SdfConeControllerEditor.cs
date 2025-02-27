using me.tooster.sdf.Editor.Controllers.SDF;
using me.tooster.sdf.Editor.Controllers.SDF.Primitives;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.Editors.PrimitiveControllers {
    [CustomEditor(typeof(SdfConeController), true)]
    class SdfConeControllerEditor : SdfPrimitiveControllerEditor {
        SerializedProperty angleProperty;
        SerializedProperty heightProperty;

        SphereBoundsHandle radiusHandle = new() { axes = PrimitiveBoundsHandle.Axes.All ^ PrimitiveBoundsHandle.Axes.Y };

        protected override void OnEnable() {
            base.OnEnable();
            // FIXME: don't depend on hidden internal property, maybe use source generators, expressions or expose an API 
            angleProperty = serializedObject.FindProperty("angle");
            heightProperty = serializedObject.FindProperty("height");
        }

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfConeController)target;


            using var drawingScope =
                new Handles.DrawingScope(Matrix4x4.TRS(controller.transform.position, controller.transform.rotation, Vector3.one));
            using var scope = new EditorGUI.ChangeCheckScope();

            radiusHandle.radius = controller.Height * Mathf.Tan(controller.Angle);
            radiusHandle.center = controller.ConeOrigin == SdfConeController.OriginPosition.TIP
                ? Vector3.up * -controller.Height
                : Vector3.zero;

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

            controller.Angle = angleProperty.floatValue = Mathf.Atan2(radiusHandle.radius, tipPosition.y);
            controller.Height = heightProperty.floatValue = tipPosition.y;

            serializedObject.ApplyModifiedProperties();
        }


        [MenuItem("GameObject/SDF/Primitives/Cone")]
        public static void Instantiate() => SdfPrimitiveController.InstantiatePrimitive<SdfConeController>("cone");
    }
}
