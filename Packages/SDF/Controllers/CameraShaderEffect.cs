using System;
using UnityEditor.Rendering;
using UnityEngine;

namespace Controllers {
    [ExecuteInEditMode]
    public class CameraShaderEffect : MonoBehaviour {
        public Material sdfSharedMaterial;
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source,destination,sdfSharedMaterial);
        }

        public void OnEnable() {
            Debug.Log(Camera.allCameras);
        }
    }
}
