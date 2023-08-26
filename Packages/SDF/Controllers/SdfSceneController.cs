#nullable enable
using System.Linq;
using API;
using Assets;
using Assets.Nodes;
using Assets.Nodes.MasterNodes;
using UnityEditor;
using UnityEngine;

namespace Controllers {
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

        // FIXME: implement asset importer and support material, shader and graph through scripted object 
        public Graph Graph { get; private set; }

        // public SdfScene? sdfScene;
        [SerializeField] private Shader   shaderAsset   = null!;
        [SerializeField] private Material materialAsset = null!;

        // TODO: a target selector

        private void Awake() {
            InitGraph();
            shaderAsset ??= ShaderUtil.CreateShaderAsset(Graph.BuildActiveTarget());
            materialAsset ??= new Material(shaderAsset);
        }

        protected void OnEnable() {
            Renderer = GetComponent<MeshRenderer>();

            Renderer.sharedMaterial = materialAsset;

            var controllers = GetComponentsInChildren<Controller>();
            var nodes = controllers.SelectMany(c => c.CollectNodes()).ToHashSet();
        }

        public void InitGraph() {
            var targetNode = new BuiltInTargetNode("built-in");
            var v_in = new VertexInNode();
            var v2f = new BasicVertToFragNode(v_in.position, null, null, null);
            var f_out = new UnlitFragOutNode(v2f.color);
            Graph = new Graph(name, new Node[] { targetNode, v_in, v2f, f_out });
        }

        // fixme: move to SO
        public void RebuildShader() {
            var shaderText = Graph.BuildActiveTarget();
            Debug.LogFormat("Shader code:\n---\n{0}\n---", shaderText);
            shaderAsset = ShaderUtil.CreateShaderAsset(shaderText);
            materialAsset.shader = shaderAsset;
        }

        // TODO: add shortcut accelerators to this and nodes (when sdf editing is enabled)
        [MenuItem("GameObject/SDF/Scene")]
        private static void CreateSdfScene() {
            var scene = GameObject.CreatePrimitive(PrimitiveType.Cube);
            scene.name = "SDF Scene";
            scene.AddComponent<SdfSceneController>();
        }
    }

    [CustomEditor(typeof(SdfSceneController))]
    public class SDfSceneControllerEditor : Editor {
        public override void OnInspectorGUI() {
            var controller = (SdfSceneController)target;

            if (GUILayout.Button("Rebuild shader")) controller.RebuildShader();
            if (GUILayout.Button("Init graph")) controller.InitGraph();

            GUILayout.Space(16);

            base.OnInspectorGUI();
        }
    }
}
