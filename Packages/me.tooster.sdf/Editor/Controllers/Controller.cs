#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using me.tooster.sdf.Editor.API;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers {
    public abstract class Controller : MonoBehaviour {
        protected SdfSceneController sdfDomain = null!;

        protected virtual void Start()  { sdfDomain = GetComponentInParent<SdfSceneController>(); }
        protected virtual void Update() { UpdateUniforms(); }

        // fixme: maybe use events instead of this - "onTransformChanged" etc. - but how to maryr it with properties?
        protected virtual void UpdateUniforms() { }


        /// <summary>
        /// Collects all variables present on the node that should be exposed on the material as properties
        /// </summary>
        public static List<Property> CollectProperties(Object o) {
            return o.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.FieldType.IsSubclassOf(typeof(Property)))
                .Select(f => (Property)f.GetValue(o))
                .ToList();
        }

        // TODO: remove or replace with getter instead of attribute?
        public static ISet<string> CollectIncludes(Object o) {
            return o.GetType().GetCustomAttributes(typeof(ShaderIncludeAttribute), true)
                .Cast<ShaderIncludeAttribute>()
                .SelectMany(attr => attr.ShaderIncludes)
                .ToHashSet();
        }

        // TODO: remove or replace with getter instead of attribute?
        public static ISet<string> CollectDefines(Object o) {
            // return values of static fields annotated with ShaderDefineAttribute
            return o.GetType().GetFields(BindingFlags.Static)
                .Where(info => info.GetCustomAttributes<ShaderGlobalAttribute>().Any())
                .Select(field => (string)field.GetValue(o))
                .ToHashSet();
        }
    }
}
