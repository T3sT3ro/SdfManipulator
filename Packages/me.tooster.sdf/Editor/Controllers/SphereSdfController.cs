using System;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.Nodes.SdfNodes;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers {
    public class SphereSdfController : TransformController {
        Property<float> radius = new Property<float>("R", "Radius", 1f);
        
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
