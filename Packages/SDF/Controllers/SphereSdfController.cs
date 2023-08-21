using UnityEditor;
using UnityEngine;

namespace Controllers {
    
    public class SphereSdfController : TransformSdfController {

        [MenuItem("GameObject/SDF/Sphere")]
        public static void CreateSDFSphere() {
            var sdf = new GameObject("sphere");
            sdf.AddComponent<SphereSdfController>();
            
            if(Selection.activeObject )
            
            sdf.transform.parent = 
            sdfSphere.name = "SDF Sphere";
        }
    }
}
