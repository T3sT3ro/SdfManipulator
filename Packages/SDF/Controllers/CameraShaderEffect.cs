using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace Controllers {
    // See SceneViewFilter.cs on how to render scene view raymarching
    [ExecuteInEditMode]
    public class CameraShaderEffect : MonoBehaviour {
        public Material sdfSharedMaterial;
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source,destination,sdfSharedMaterial);
        }

        [MenuItem("GameObject/Print Camera Position"), ]
        private void Update() {
            var camTransformt = SceneView.GetAllSceneCameras()[0].transform;
            Debug.Log(String.Format("pos:{0} rot:{1}", camTransformt.position, camTransformt.rotation));
        }
    }
}
