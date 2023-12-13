using System;
using me.tooster.sdf.Editor.NodeGraph.Nodes.SdfNodes;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers {
    public class SphereSdfController : TransformController {
        private SdfSphereNode sphereNode;

        protected override void Start() {
            sphereNode ??= new SdfSphereNode(null, null);
        }

        // TODO: add accelerator
        [MenuItem("GameObject/SDF/Sphere", priority=-20)]
        public static void CreateSDFSphere() {
            var target = Selection.activeGameObject;
            var scene = target.GetComponentInParent<SdfSceneController>();
            if (scene == null)
                throw new Exception("Primitives must be added under Sdf Scene Controller");
            
            var sdf = new GameObject("sphere");
            var controller = sdf.AddComponent<SphereSdfController>();

            if (Selection.activeObject)
                sdf.transform.SetParent(target.transform);
        }
    }
}
