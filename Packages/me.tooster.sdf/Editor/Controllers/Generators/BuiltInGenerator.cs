using System;
using System.Collections.Generic;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Shaderlab;
using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.Generators {
    public partial class BuiltInGenerator : RaymarchingShaderGenerator {
        public BuiltInGenerator(SdfScene scene) : base(scene) { }

        readonly HashSet<string> includeFiles = new()
        {
            "Packages/me.tooster.sdf/Editor/Resources/Includes/raymarching.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/util.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/matrix.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl",
            // "Packages/me.tooster.sdf/Editor/Resources/Includes/builtInForwardBase.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/debugBaseShading.hlsl",
        };
        readonly HashSet<FunctionDefinition> functionDefinitions = new();

        [SerializeField] bool transparent = false;

        public override string MainShader() {
            if (scene.sdfSceneRoot == null) return "// empty shader";
            var unformatedSource = shader();
            var formattedSource = ShaderlabFormatter.Format(unformatedSource);
            return formattedSource?.ToString() ?? "// empty shader";
        }


        public override void HandleRequirement(Requirement requirement) {
            switch (requirement) {
                case IncludeRequirement includeRequirement:
                    includeFiles.Add(includeRequirement.FileName);
                    break;
                case FunctionDefinitionRequirement functionDefinitionRequirement:
                    functionDefinitions.Add(functionDefinitionRequirement.functionDefinition);
                    break;
                default:
                    throw new InvalidOperationException($"Unhandled requirement {requirement}");
            }
        }

        // TODO: do it some other way (was OnValidate, testing Reset currently), because those includes can be deleted
        // in editor and due to ScriptableSingleton can be persisted between reloads.
        // they also crash sometimes
        // void Reset() => ReassignDependencies();
        //
        // void OnEnable() {
        //     ReassignDependencies();
        //     hideFlags = HideFlags.DontSave;
        // }
        //
        // void OnValidate() { EditorApplication.delayCall += ReassignDependencies; }
        //
        // void ReassignDependencies() {
        //     dependentIncludes = dependentIncludePaths.Select(AssetDatabase.LoadAssetAtPath<ShaderInclude>).ToArray();
        // }
    }
}
