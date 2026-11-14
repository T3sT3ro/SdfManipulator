using me.tooster.sdf.Primitives;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace me.tooster.sdf.Editor.Inspectors.Primitives {
    [CustomEditor(typeof(SdfConeController), true)]
    class SdfConeControllerEditor : SdfPrimitiveControllerEditor {
        readonly SphereBoundsHandle radiusHandle = new() { axes = PrimitiveBoundsHandle.Axes.All ^ PrimitiveBoundsHandle.Axes.Y };

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfConeController)target;


            using var drawingScope = new Handles.DrawingScope(controller.transform.localToWorldMatrix);
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

            if (!scope.changed)
                return;

            Undo.RecordObject(controller, "Resize cone");
            controller.Angle = Mathf.Atan2(radiusHandle.radius, tipPosition.y);
            controller.Height = tipPosition.y;
        }


        [MenuItem("GameObject/SDF/Primitives/Cone")]
        static void Instantiate(MenuCommand command) => SdfPrimitiveControllerEditor.InstantiatePrimitive<SdfConeController>("cone", command);
    }
}
