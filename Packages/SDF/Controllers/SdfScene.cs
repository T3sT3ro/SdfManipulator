using BuiltInTarget;
using UnityEditor;
using UnityEngine;

namespace Controllers {
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [SelectionBase]
    public class SdfScene : MonoBehaviour {
        private Graph graph;

        [SerializeField] private Material material;
        [SerializeField] private Shader   shader;

        private void OnEnable() {
            if (shader == null) { GenerateShader(); }
            
            var material = new Material(shader);
            AssetDatabase.CreateAsset(material, "Test/GeneratedShaders/SimpleShader.mat");
        }

        private void GenerateShader() {
            var graphBuilder = new GraphBuilder(new UnlitShaderProgram(), graph);
            shader = ShaderUtil.CreateShaderAsset();
            
            AssetDatabase.CreateAsset(shader, "Test/GeneratedShaders/SimpleShader.hlsl");
        }

        [MenuItem("GameObject/SDF/Scene")]
        private static void CreateSdfScene() {
            var scene = new GameObject("SDF Scene");

            scene.AddComponent<SdfScene>();
        }
    }
}
