using System;
using System.Linq;
using UnityEditor;

namespace me.tooster.sdf.Editor.Controllers.ShaderPartials {
    // TODO: refactor into builder + SO and cache partially generated syntax
    public partial class RaymarchingShader : ScriptableSingleton<RaymarchingShader> {
        public ShaderInclude[] dependentIncludes = null!;

        private static string[] dependentIncludePaths => new[]
        {
            "Packages/me.tooster.sdf/Editor/Resources/Includes/types.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/util.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/matrix.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/noise.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/raymarching.hlsl",
        };

        private RaymarchingShader() { }

        private void OnEnable() { ReassignDependencies(); }

        private void OnValidate() {
            // TODO: do it some other way, because those includes can be deleted
            // in editor and due to ScriptableSingleton can be persisted between reloads.
            // they also crash sometimes
            EditorApplication.delayCall += ReassignDependencies;
        }

        private void ReassignDependencies() {
            dependentIncludes = dependentIncludePaths.Select(AssetDatabase.LoadAssetAtPath<ShaderInclude>).ToArray();
        }
    }
}
