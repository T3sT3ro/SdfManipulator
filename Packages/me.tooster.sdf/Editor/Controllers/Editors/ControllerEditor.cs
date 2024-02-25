#nullable enable
using System;
using System.Collections.Generic;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    [CustomEditor(typeof(Controller), true)]
    public class ControllerEditor : UnityEditor.Editor {
        private Dictionary<Property, object> lastPropertyValues = new();

        public override void OnInspectorGUI() {
            EditorGUILayout.LabelField("Custom SDF controller", EditorStyles.boldLabel);
            var controller = (Controller)target;
            if (controller.SdfScene == null) {
                EditorGUILayout.HelpBox("This node doesn't belong to any scene", MessageType.Error);
                return;
            }

            base.OnInspectorGUI();

            foreach (var prop in controller.Properties) {
                EditorGUILayout.LabelField(prop.DisplayName, EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Last value: {lastPropertyValues[prop]}");
            }
        }

        private void Awake() {
            var controller = (Controller)target;
            foreach (var prop in controller.Properties) {
                prop.onValueChanged += propertyValueChanged;
                lastPropertyValues[prop] = default;
            }
        }


        private void propertyValueChanged(Property property, object value) { lastPropertyValues[property] = value; }

        private void OnSceneGUI() {
            var controller = (Controller)target;
            var tr = controller.transform;
            var pos = tr.localPosition;
            var size = HandleUtility.GetHandleSize(pos) * 0.5f;
            var snap = Vector3.one * 0.5f;

            EditorGUI.BeginChangeCheck();
            var newTargetPosition = Handles.FreeMoveHandle(pos, size, snap, Handles.RectangleHandleCap);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(controller, "Change Look At Target Position");
                tr.localPosition = newTargetPosition;
            }
        }
    }
}
