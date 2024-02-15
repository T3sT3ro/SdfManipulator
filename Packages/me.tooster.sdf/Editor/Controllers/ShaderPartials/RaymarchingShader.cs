using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers.ShaderPartials {
    // TODO: refactor into builder + SO and cache partially generated syntax
    public partial class RaymarchingShader : ScriptableSingleton<RaymarchingShader> {
        public ShaderInclude[] dependentIncludes = null!;

        private static string[] dependentIncludePaths => new[]
        {
            "Packages/me.tooster.sdf/Editor/Resources/Includes/raymarching.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/util.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/matrix.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/noise.hlsl",
        };

        private RaymarchingShader() { }

        private void OnEnable() {
            ReassignDependencies();
            hideFlags = HideFlags.DontSave;
        }

        private void Reset() =>
            // TODO: do it some other way (was OnValidate, testing Reset currently), because those includes can be deleted
            // in editor and due to ScriptableSingleton can be persisted between reloads.
            // they also crash sometimes
            ReassignDependencies();

        private void OnValidate() { EditorApplication.delayCall += ReassignDependencies; }

        private void ReassignDependencies() {
            dependentIncludes = dependentIncludePaths.Select(AssetDatabase.LoadAssetAtPath<ShaderInclude>).ToArray();
        }
    }
}
