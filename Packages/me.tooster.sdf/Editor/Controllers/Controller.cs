#nullable enable
using System;
using System.Collections.Generic;
using me.tooster.sdf.Editor.API;
using UnityEditor;
using UnityEngine;
using Property = me.tooster.sdf.Editor.API.Property;

namespace me.tooster.sdf.Editor.Controllers {
    /**
     * TODO: refactor to a editor + runtime type with conditional compilation.
     * Runtime would handle updating uniforms/keywords while editor handle updating the material and shader.
     */
    [Icon("Packages/me.tooster.sdf/Editor/Resources/Icons/sdf-icon-64x64.png")]
    public abstract class Controller : MonoBehaviour, IShaderPartialProvider {
        public          SdfScene?             SdfScene   { get; internal set; } = null!;
        public abstract IEnumerable<Property> Properties { get; }
        public          Vector3               targetPosition = Vector3.zero;

        public void Update() { }

        private void OnValidate() {
            // TODO: make this have effect only in Editor, not in runtime.
            SdfScene = GetComponentInParent<SdfScene>();
        }

        private void OnDrawGizmos() {
            Gizmos.DrawIcon(transform.position, "Packages/me.tooster.sdf/Editor/Resources/Icons/sdf-icon-256.png", true);
        }

        private void OnDrawGizmosSelected() {
            if (Selection.activeGameObject != gameObject) return;

            Gizmos.color = Color.magenta;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, transform.localScale);
        }

        public static void TryCreateNode<TController>(string name) where TController : Controller, new() {
            var target = Selection.activeGameObject;

            var sdf = new GameObject(name);
            var controller = sdf.AddComponent<TController>();

            if (!target) return;

            sdf.transform.SetParent(Selection.activeTransform);
            target.GetComponentInParent<SdfScene>().AttachController(controller);
        }
    }
}
