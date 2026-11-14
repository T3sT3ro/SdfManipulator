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

        /// <summary>Creates an SDF object with a transform, the primitive <typeparamref name="TController" /> and an SDF controller.</summary>
        public static GameObject InstantiatePrimitive<TController>(string name, MenuCommand command = null) where TController : SdfPrimitiveController
            => CreateSdfObject(
                name,
                command,
                go => {
                    var transformController = go.AddComponent<TransformController>();
                    var primitive = go.AddComponent<TController>();
                    go.AddComponent<SdfController>().sdfModifiers = new Controller[] { transformController, primitive };
                }
            );

        public override VisualElement CreateInspectorGUI() {
            var root = base.CreateInspectorGUI();

            return root;
        }
    }
}
