using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers {
    // See SceneViewFilter.cs on how to render scene view raymarching
    [ExecuteInEditMode]
    public class CameraShaderEffect : MonoBehaviour {
        public Material sdfSharedMaterial;
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source,destination,sdfSharedMaterial);
        }

        // logs editor camera coordinates
        [MenuItem("GameObject/Print Camera Position")]
        private void Update() {
            var camTransformt = SceneView.GetAllSceneCameras()[0].transform;
            Debug.Log($"CAMERA pos:{camTransformt.position} rot:{camTransformt.rotation}");
        }
    }
}
