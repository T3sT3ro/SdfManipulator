using System;
using System.Collections.Generic;
using me.tooster.sdf.Editor.API;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers {
    public class SphereSdfController : TransformController {
        readonly Property<float> radius = new Property<float>("radius", "Sphere radius", 1f);

        public override IEnumerable<Property> Properties {
            get { yield return radius; }
        }

        // TODO: add accelerator
        [MenuItem("GameObject/SDF/Primitives/Sphere", priority = -20)]
        public static void CreateSdfSphere() {
            var target = Selection.activeGameObject;
            var scene = target.GetComponentInParent<SdfScene>();
            if (scene == null)
                throw new Exception("Primitives must be added under Sdf Scene Controller");

            var sdf = new GameObject("sphere");
            var controller = sdf.AddComponent<SphereSdfController>();

            if (Selection.activeObject)
                sdf.transform.SetParent(target.transform);
        }
        
        [MenuItem("GameObject/SDF/Primitives/Sphere", priority = -20)]
        public static void CreateSdfBox() => Controller.TryCreateNode<SphereSdfController>("box");
    }
}
