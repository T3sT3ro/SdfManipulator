using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;
namespace me.tooster.sdf.Editor.GraphEditor.Editor.GraphEditor {
    [Graph(AssetExtension, GraphOptions.SupportsSubgraphs)]
    [Serializable]
    public class T3ShaderGraph : Graph {
        public const string AssetExtension = "t3graph";

        [MenuItem("Assets/Create/SDF/Shader Graph", priority = 201)]
        static void CreateAssetFile() { GraphDatabase.PromptInProjectBrowserToCreateNewAsset<T3ShaderGraph>(); }
    }
}
