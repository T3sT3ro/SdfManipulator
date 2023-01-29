using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SDF {
    
    public class MasterShaderNode : MonoBehaviour {
        
        private static Uri generatedShadersRoot = new Uri("SDF/Generated");

        [SerializeField] private Uri generatedShaderPath = new Uri(generatedShadersRoot, "output.shader");
        
        void updateShader() {

            var existingShader = AssetDatabase.LoadAssetAtPath<Shader>(generatedShaderPath.AbsolutePath);
            if (!existingShader) {
                AssetDatabase.CreateAsset(Shader.Instantiate(null), generatedShaderPath.AbsolutePath);
            }
        }
    }
}
