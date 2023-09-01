using UnityEngine;

// scene view rendering of fullscreen raymarching objects
namespace me.tooster.sdf.Editor {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class SDFRenderer : SceneViewFilter {
        public Material material;
        
        private void OnRenderImage(RenderTexture source, RenderTexture destination) {
            if (material == null) {
                Graphics.Blit(source, destination);
                return;
            }
            
            Graphics.Blit(source, destination, material);
        }
    }
}
