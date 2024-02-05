#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations;
using me.tooster.sdf.Editor.API;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Property = me.tooster.sdf.Editor.API.Property;

namespace me.tooster.sdf.Editor.Controllers {
    /**
     * TODO: refactor to a editor + runtime type with conditional compilation.
     * Runtime would handle updating uniforms/keywords while editor handle updating the material and shader.
     */
    public abstract class Controller : MonoBehaviour, IShaderPartialProvider {
        public          SdfScene?             SdfScene   { get; internal set; } = null!;
        public abstract IEnumerable<Property> Properties { get; }

        private void OnValidate() {
            // TODO: make this have effect only in Editor, not in runtime.
            SdfScene = GetComponentInParent<SdfScene>();
        }

        public static void TryCreateNode<TController>(string name) where TController : Controller, new() {
            var target = Selection.activeGameObject;

            var sdf = new GameObject(name);
            var controller = sdf.AddComponent<TController>();

            if (!target) return;

            sdf.transform.SetParent(Selection.activeTransform);
            target.GetComponentInParent<SdfScene>().AttachController(controller);
        }

        /// <summary>
        /// Collects all variables present on the node that should be exposed on the material as properties
        /// </summary>
        [Obsolete("this was taken from the previous graph model, not needed for now")]
        public static IEnumerable<Property> CollectProperties(Object o) {
            return o.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.FieldType.IsSubclassOf(typeof(Property)))
                .SelectMany(f => (IEnumerable<Property>)f.GetValue(o))
                .Where(property => property is not null);
        }

        // TODO: remove or replace with getter instead of attribute?
        public static ISet<string> CollectIncludes(Object o) {
            return o.GetType().GetCustomAttributes(typeof(ShaderIncludeAttribute), true)
                .Cast<ShaderIncludeAttribute>()
                .SelectMany(attr => attr.ShaderIncludes)
                .Where(property => property is not null)
                .ToHashSet();
        }

        // TODO: remove or replace with getter instead of attribute?
        public static ISet<string> CollectDefines(Object o) {
            // return values of static fields annotated with ShaderDefineAttribute
            return o.GetType().GetFields(BindingFlags.Static)
                .Where(info => info.GetCustomAttributes<ShaderGlobalAttribute>().Any())
                .Select(field => (string)field.GetValue(o))
                .Where(property => property is not null)
                .ToHashSet();
        }
    }

    [CustomEditor(typeof(Controller))]
    public class ControllerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            if (target is Controller { SdfScene: null }) // show warning if no SdfScene is assigned
                EditorGUILayout.HelpBox("This node doesn't belong to any scene", MessageType.Error);
            base.OnInspectorGUI();
        }
    }
}
