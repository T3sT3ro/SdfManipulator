#nullable enable
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
    public abstract class Controller : MonoBehaviour, IShaderPartialProvider {
        public          SdfScene?             SdfScene   => GetComponentInParent<SdfScene>(true);
        public abstract IEnumerable<Property> Properties { get; }

        private void OnDrawGizmos() {
            Gizmos.DrawIcon(transform.position, "Packages/me.tooster.sdf/Editor/Resources/Icons/sdf-icon-256.png", true);
        }

        private void OnDrawGizmosSelected() {
            if (Selection.activeGameObject != gameObject) return;

            Gizmos.color = Color.magenta;
            var tr = transform;
            Gizmos.matrix = tr.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, tr.localScale);
        }

        public static void TryCreateNode<TController>(string name) where TController : Controller, new() {
            var target = Selection.activeGameObject;

            var sdf = new GameObject(name);
            var controller = sdf.AddComponent<TController>();

            if (!target) return;

            sdf.transform.SetParent(Selection.activeTransform);
            target.GetComponentInParent<SdfScene>()?.RegisterController(controller);
        }
    }
}
