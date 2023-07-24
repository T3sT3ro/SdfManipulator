using System;
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
    }
}
