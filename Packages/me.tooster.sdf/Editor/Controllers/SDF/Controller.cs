#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Util;
using UnityEditor;
using UnityEngine;
using Property = me.tooster.sdf.Editor.API.Property;

namespace me.tooster.sdf.Editor.Controllers.SDF {
    /**
     * TODO: refactor to a editor + runtime type with conditional compilation.
     * Runtime would handle updating uniforms/keywords while editor handle updating the material and shader.
     */
    [Icon("Packages/me.tooster.sdf/Editor/Resources/Icons/sdf-icon-256.png")]
    [DisallowMultipleComponent]
    public abstract class Controller : MonoBehaviour, IShaderPartialProvider {
        // TODO: cache it, register and unregister properly
        public          SdfScene?             SdfScene   => GetComponentInParent<SdfScene>(true);
        public abstract IEnumerable<Property> Properties { get; }

        private void OnValidate() {
            var tr = transform;
            tr.hideFlags |= HideFlags.HideInInspector;
        }

        public static void TryCreateController<TController>(string name) where TController : Controller, new() {
            var target = Selection.activeGameObject;

            var sdf = new GameObject(name);
            var controller = sdf.AddComponent<TController>();

            if (!target) return;

            sdf.transform.SetParent(Selection.activeTransform);
        }

        private void OnTransformParentChanged() {
            NotifyStructureChanged();
            var parentController = transform.parent.GetComponent<Controller>();
            if (parentController)
                onStructureChanged = parentController.onStructureChanged;
            if (SdfScene is { } scene) scene.Register(this);
            SdfScene.QueuePropertyUpdates(Properties);
        }

        public delegate void StructureChanged(Controller source);

        public event StructureChanged onStructureChanged = delegate { };

        public void NotifyStructureChanged() => onStructureChanged(this);
    }


    [CustomEditor(typeof(Controller), true)]
    [CanEditMultipleObjects]
    public class ControllerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            var controller = (Controller)target;
            if (controller.SdfScene == null) {
                EditorGUILayout.HelpBox("This node doesn't belong to any scene", MessageType.Error);
                return;
            }

            base.OnInspectorGUI();

            foreach (var prop in controller.Properties) {
                EditorGUILayout.LabelField(prop.DisplayName, EditorStyles.boldLabel);
                if (GUILayout.Button("trigger update for this property"))
                    controller.SdfScene.QueuePropertyUpdates(prop);
                EditorGUILayout.TextArea(prop.CurrentValue.ToString());
            }
        }

        protected virtual void OnSceneGUI() {
            var controller = (Controller)target;
            var tr = controller.transform;
            var pos = tr.position;
            var size = HandleUtility.GetHandleSize(pos) * 0.5f;
            var snap = Vector3.one * 0.5f;

            using (var scope = new EditorGUI.ChangeCheckScope()) {
                var targetPos = Handles.FreeMoveHandle(pos, size, snap, Handles.CircleHandleCap);
                if (!scope.changed) return;

                Undo.RecordObject(controller.transform, "Freemovee Controller");
                tr.position = targetPos;
            }
        }
    }
}
