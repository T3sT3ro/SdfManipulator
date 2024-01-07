#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Shaderlab;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.ShaderPartials;
using me.tooster.sdf.Editor.NodeGraph.Nodes;
using me.tooster.sdf.Editor.NodeGraph.Nodes.MasterNodes;
using UnityEditor;
using UnityEngine;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;
using VariableDeclarator = me.tooster.sdf.AST.Hlsl.Syntax.VariableDeclarator;

namespace me.tooster.sdf.Editor.Controllers {
    /// <summary>
    /// SdfSceneController is a game object that handles displaying and controling SdfScene asset. 
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [DisallowMultipleComponent]
    [SelectionBase]
    [ExecuteAlways]
    public class SdfSceneController : Controller {
        public Renderer Renderer = null!;
        public Material Material => Renderer.sharedMaterial;

        // public SdfScene? sdfScene;
        [SerializeField] private Shader?   shaderAsset;
        [SerializeField] private Material? materialAsset;

        // TODO: a target selector

        private void OnValidate() {
            if (shaderAsset == null) shaderAsset = ShaderUtil.CreateShaderAsset("// empty shader");
            if (materialAsset == null) materialAsset = new Material(shaderAsset);
            materialAsset.name = shaderAsset.name;
        }

        protected void OnEnable() {
            Renderer = GetComponent<MeshRenderer>();

            Renderer.sharedMaterial = materialAsset;

            var controllers = GetComponentsInChildren<Controller>();
        }

        // fixme: move to ScriptableObject
        public void RebuildShader() {
            var shaderText = AssembleShaderSource();
            Debug.LogFormat("Shader code:\n---\n{0}\n---", shaderText);
            ShaderUtil.UpdateShaderAsset(shaderAsset, shaderText);
        }

        // collect all "Properties" in all children components
        // TODO: make it return cached properties, update them when children are changed
        public IEnumerable<Property> Properties => GetComponentsInChildren<Controller>()
            .SelectMany(CollectProperties);

        // TODO: add shortcut accelerators to this and nodes (when sdf editing is enabled)
        [MenuItem("GameObject/SDF/Scene")]
        private static void CreateSdfScene() {
            var scene = GameObject.CreatePrimitive(PrimitiveType.Cube);
            scene.name = "SDF Scene";
            scene.AddComponent<SdfSceneController>();
        }

        protected virtual string AssembleShaderSource() { return RaymarchingShader.MainShader(this); }
    }

    [CustomEditor(typeof(SdfSceneController))]
    public partial class SDfSceneControllerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            var controller = (SdfSceneController)target;

            if (GUILayout.Button("Rebuild shader")) controller.RebuildShader();
            // if (GUILayout.Button("Test Init graph")) controller.InitGraph();
            GUILayout.Space(16);

            base.OnInspectorGUI();
        }
    }
}
