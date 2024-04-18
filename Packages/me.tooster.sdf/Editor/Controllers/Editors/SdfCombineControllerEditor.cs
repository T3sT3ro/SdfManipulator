using System;
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
            foreach (var child in controller.SdfControllers) {
                Handles.color = GetGizmoColor(controller);
                Handles.DrawLine(controller.transform.position, child.transform.position, 8);
                Handles.color = Color.black;
                Handles.DrawLine(controller.transform.position, child.transform.position, 6);
                Handles.color = base.GetGizmoColor(child);
                Handles.DrawDottedLine(controller.transform.position, child.transform.position, 4);
            }
        }

        protected override Color GetGizmoColor(SdfController controller)
            => ((SdfCombineController)controller).Operation switch
            {
                SdfCombineController.CombinationOperation.SIMPLE_UNION => Color.blue,
                SdfCombineController.CombinationOperation.INTERSECTION => Color.yellow,
                SdfCombineController.CombinationOperation.SMOOTH_UNION => Color.cyan,
                _                                                      => throw new ArgumentOutOfRangeException(),
            };


        [MenuItem("GameObject/SDF/Operators/Combine", priority = 200)]
        public static void Instantiate() => Controller.TryInstantiate<SdfCombineController>("combine");
    }
}
