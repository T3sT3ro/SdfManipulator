using System.Linq;
using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    [CustomEditor(typeof(SdfCombineController))]
    public class SdfCombineControllerEditor : SdfControllerEditor {
        protected override void OnSceneGUI() {
            base.OnSceneGUI();

            var controller = (SdfCombineController)target;
            using var scope = new EditorGUI.ChangeCheckScope();
            foreach (var child in controller.SdfControllers.OfType<SdfController>()) {
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

        protected override Color GetGizmoColor(SdfController controller)
            => controller switch
            {
                SdfCombineController { Operation: SdfCombineController.CombinationOperation.SIMPLE_UNION } => Color.blue,
                SdfCombineController { Operation: SdfCombineController.CombinationOperation.INTERSECTION } => Color.yellow,
                SdfCombineController { Operation: SdfCombineController.CombinationOperation.SMOOTH_UNION } => Color.cyan,
                _ => base.GetGizmoColor(controller),
            };


        [MenuItem("GameObject/SDF/Operators/Combine", priority = 200)]
        public static void Instantiate() => Controller.TryInstantiate<SdfCombineController>("combine");
    }
}
