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
        [SerializeField] private Shader            shaderAsset       = null!;
        [SerializeField] private Material          materialAsset     = null!;
        [SerializeField] private RaymarchingShader raymarchingShader = null!;

        // TODO: a target selector

        private void Awake() {
            // InitGraph();
            shaderAsset ??= ShaderUtil.CreateShaderAsset("// empty shader");
            materialAsset ??= new Material(shaderAsset);
        }

        protected void OnEnable() {
            Renderer = GetComponent<MeshRenderer>();

            Renderer.sharedMaterial = materialAsset;

            var controllers = GetComponentsInChildren<Controller>();
        }

        public void InitGraph() {
            var v_in = new VertexInNode();
            var v2f = new BasicVertToFragNode(v_in.position, v_in.normal, v_in.uv, null);
            var f_out = new UnlitFragOutNode(v2f.color);
            var targetNode = new BuiltInTargetNode("built-in", v_in, v2f, f_out);
        }

        // fixme: move to ScriptableObject
        public void RebuildShader() {
            var shaderText = raymarchingShader.MainShader();
            Debug.LogFormat("Shader code:\n---\n{0}\n---", shaderText);
            shaderAsset = ShaderUtil.CreateShaderAsset(shaderText);
            materialAsset.shader = shaderAsset;
        }

        // collect all "Properties" in all children components
        // TODO: make it return cached properties, update them when children are changed
        public IEnumerable<Property> Properties => GetComponentsInChildren<Controller>()
            .SelectMany(c => c.CollectProperties());

        // TODO: add shortcut accelerators to this and nodes (when sdf editing is enabled)
        [MenuItem("GameObject/SDF/Scene")]
        private static void CreateSdfScene() {
            var scene = GameObject.CreatePrimitive(PrimitiveType.Cube);
            scene.name = "SDF Scene";
            scene.AddComponent<SdfSceneController>();
        }
    }

    [CustomEditor(typeof(SdfSceneController))]
    public partial class SDfSceneControllerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            var controller = (SdfSceneController)target;

            if (GUILayout.Button("Rebuild shader")) controller.RebuildShader();
            if (GUILayout.Button("Test Init graph")) controller.InitGraph();
            GUILayout.Space(16);

            base.OnInspectorGUI();
        }
    }
}
