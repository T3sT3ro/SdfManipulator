using me.tooster.sdf.Primitives;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace me.tooster.sdf.Editor.Inspectors.Primitives {
    [CustomEditor(typeof(SdfPrimitiveController), true)]
    public class SdfPrimitiveControllerEditor : ControllerEditor {
        SdfController parentSdfObject;

        protected virtual void OnEnable() { parentSdfObject = (target as SdfPrimitiveController)?.GetComponent<SdfController>(); }

        protected virtual void OnSceneGUI() { Gizmos.color = Handles.color = GetGizmoColor(target as Controller); }

        protected virtual Color GetGizmoColor(Controller controller) {
            if (parentSdfObject != null)
                return parentSdfObject.Inverted ? Color.red : Color.green;
            return Color.white;
        }

        public static void InstantiatePrimitive<TController>(string name) where TController : SdfPrimitiveController, new() {
            Undo.IncrementCurrentGroup();

            var sdf = new GameObject(name);

            Undo.RegisterCreatedObjectUndo(sdf, $"Create {name}");

            ObjectNames.SetNameSmart(sdf, name);
            var target = Selection.activeGameObject;

            var transformController = sdf.AddComponent<TransformController>();
            var primitiveSdfController = sdf.AddComponent<TController>();
            var sdfObjectController = sdf.AddComponent<SdfController>();

            sdfObjectController.sdfModifiers = new Controller[] { transformController, primitiveSdfController };

            // call this after adding components to properly filter in OnTransformChildrenChange
            if (target)
                sdf.transform.SetParent(Selection.activeTransform);
            sdf.transform.localPosition = Vector3.zero;

            Undo.RegisterCompleteObjectUndo(sdf, $"Create {name}");

            Selection.activeGameObject = sdf;

            Undo.SetCurrentGroupName($"Create a primitive SDF {name}");
        }

        public override VisualElement CreateInspectorGUI() {
            var root = base.CreateInspectorGUI();

            return root;
        }
    }
}
