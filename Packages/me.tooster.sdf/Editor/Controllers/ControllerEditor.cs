#nullable enable
using System;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers {
    [CustomEditor(typeof(Controller), true)]
    public class ControllerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            EditorGUILayout.LabelField("Custom SDF controller", EditorStyles.boldLabel);
            if (target is Controller { SdfScene: null }) // show warning if no SdfScene is assigned
                EditorGUILayout.HelpBox("This node doesn't belong to any scene", MessageType.Error);
            base.OnInspectorGUI();
        }

        void OnSceneGUI() {
            var controller = (Controller) target;
            float size = HandleUtility.GetHandleSize(controller.targetPosition) * 0.5f;
            Vector3 snap = Vector3.one * 0.5f;

            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.FreeMoveHandle(controller.targetPosition, size, snap, Handles.RectangleHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(controller, "Change Look At Target Position");
                controller.targetPosition = newTargetPosition;
                controller.Update();
            }
        }
    }

}
