#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.Editor.API;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers {
    /**
     * A basic controller for sdf primitives and positionable elements.
     */
    [ExecuteInEditMode]
    [RequireComponent(typeof(Transform))]
    public class TransformController : Controller {
        // TODO: consolidate variables and properties? Should properties be responsible for their final values
        public Vector3 size = Vector3.one / 4;

        private Property<Matrix4x4> transformProperty =
            new Property<Matrix4x4>("transform", "Transform", Matrix4x4.identity);


        private void Update() {
            if (SdfScene == null) return;

            var spaceTransform = Matrix4x4.Rotate(Quaternion.Inverse(transform.localRotation));
            spaceTransform *=
                Matrix4x4.Translate(Vector3.Scale(-transform.localPosition, SdfScene.transform.localScale));

            transformProperty.UpdateValue(this, spaceTransform);
        }

        public override IEnumerable<Property> Properties {
            get { yield return transformProperty; }
        }
    }

    [UnityEditor.CustomEditor(typeof(TransformController))]
    public class TransformControllerGui : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var controller = (TransformController)target;
            Handles.color = Color.red;
            Handles.DrawWireCube(Vector3.zero, controller.size);
        }
    }
}
