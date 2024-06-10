using System.Linq;
using me.tooster.sdf.Editor.Controllers.Editors.PrimitiveControllers;
using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    [CustomEditor(typeof(SdfCombineController))]
    public class SdfCombineControllerEditor : SdfPrimitiveControllerEditor {
        public override VisualElement CreateInspectorGUI() {
            var baseInspector = base.CreateInspectorGUI();
            var controller = (SdfCombineController)target;

            // hide property field for blend if variant is other than smooth blend. Observe changes to show and hide.
            if (controller.Operation != SdfCombineController.CombinationOperation.SMOOTH_UNION)
                return baseInspector;

            var operation = baseInspector.Q<PropertyField>("operation");
            var blendField = baseInspector.Q<PropertyField>("blendFactor");

            operation.visible = false;

            operation.RegisterValueChangeCallback(
                _ => blendField.visible = controller.Operation == SdfCombineController.CombinationOperation.SMOOTH_UNION
            );

            return baseInspector;
        }

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfCombineController)target;
            using var scope = new EditorGUI.ChangeCheckScope();
            foreach (var child in controller.children.OfType<SdfController>()) {
                if (child is not Component { transform: { position: var childPos } }) continue;
                Handles.color = GetGizmoColor(controller);
                var parentPos = controller.transform.position;
                Handles.DrawLine(parentPos, childPos, 8);
                Handles.color = Color.black;
                Handles.DrawLine(parentPos, childPos, 6);
                Handles.color = base.GetGizmoColor(controller);
                Handles.DrawDottedLine(parentPos, childPos, 4);
            }
        }

        protected override Color GetGizmoColor(Controller controller)
            => controller switch
            {
                SdfCombineController { Operation: SdfCombineController.CombinationOperation.SIMPLE_UNION } => Color.green,
                SdfCombineController { Operation: SdfCombineController.CombinationOperation.INTERSECTION } => Color.yellow,
                SdfCombineController { Operation: SdfCombineController.CombinationOperation.SMOOTH_UNION } => Color.cyan,
                _ => base.GetGizmoColor(controller),
            };


        [MenuItem("GameObject/SDF/Operators/Combine", priority = 200)]
        public static void Instantiate() => InstantiateController<SdfCombineController>("combine");
    }
}
