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
        private void Update() {
            PrintPos();
        }

        [MenuItem("GameObject/Print Camera Position")]
        private static void PrintPos() {
            var camTransform = SceneView.GetAllSceneCameras()[0].transform;
            Debug.Log($"CAMERA pos:{camTransform.position} rot:{camTransform.rotation}");
        }
    }
}
