using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.ShaderPartials {
    public interface IRaymarchingShader {
        public Tree<hlsl>      commonIncludeAST { get; }
        public Tree<shaderlab> shaderAST        { get; }
        public ShaderInclude[] requiredIncludes { get; }
    }



    public abstract class RaymarchingShaderTemplate {
        public    Tree<hlsl>      commonIncludeAST;
        public    ShaderInclude[] requiredIncludes;
        protected SdfScene        sdfScene;
        public    Tree<shaderlab> shaderAST;

        public RaymarchingShaderTemplate(SdfScene sdfScene) => this.sdfScene = sdfScene;
    }



    // TODO: refactor into builder + SO and cache partially generated syntax
    public partial class RaymarchingShader : ScriptableSingleton<RaymarchingShader> {
        public ShaderInclude[] dependentIncludes = null!;

        RaymarchingShader() { }

        static string[] dependentIncludePaths => new[]
        {
            "Packages/me.tooster.sdf/Editor/Resources/Includes/raymarching.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/util.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/matrix.hlsl",
        };

        void Reset()
            =>
                // TODO: do it some other way (was OnValidate, testing Reset currently), because those includes can be deleted
                // in editor and due to ScriptableSingleton can be persisted between reloads.
                // they also crash sometimes
                ReassignDependencies();

        void OnEnable() {
            ReassignDependencies();
            hideFlags = HideFlags.DontSave;
        }

        void OnValidate() { EditorApplication.delayCall += ReassignDependencies; }

        void ReassignDependencies() {
            dependentIncludes = dependentIncludePaths.Select(AssetDatabase.LoadAssetAtPath<ShaderInclude>).ToArray();
        }
    }
}
