#nullable enable
using System;
using System.IO;
using System.Linq;
using me.tooster.sdf.Util;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor {
    /// <summary>
    ///     Generates the shader and a default material of an SDF scene whenever its prefab is imported, and adds them to the
    ///     import result as sub-assets of the prefab, the same way Shader Graph adds a shader and a default material to a
    ///     graph. They are never written into the prefab file, so regeneration doesn't change the file, and every save of
    ///     the prefab regenerates them.
    /// </summary>
    class SdfScenePostprocessor : AssetPostprocessor {
        internal const string ShaderIdentifier   = "sdf-shader";
        internal const string MaterialIdentifier = "sdf-material";

        public override uint GetVersion() => 1;

        void OnPostprocessPrefab(GameObject root) {
            // A scene in a nested prefab belongs to that prefab, which generates its own shader.
            if (!root.TryGetComponent<SdfScene>(out var scene))
                return;

            // regenerate when the generator code or the shader library changes
            context.DependsOnCustomDependency(SdfGeneratorDependency.Name);
            foreach (var include in SdfGeneratorDependency.ShaderLibraryFiles)
                context.DependsOnSourceAsset(include);

            string source;
            try {
                scene.RevalidateScene();
                source = scene.AssembleShaderSource();
            } catch (Exception e) {
                context.LogImportError($"Generating the shader of SDF scene '{root.name}' failed: {e}");
                return;
            }

            var shader = ShaderUtil.CreateShaderAsset(context, source, false);
            context.AddObjectToAsset(ShaderIdentifier, shader);

            var material = new Material(shader) { name = root.name };
            context.AddObjectToAsset(MaterialIdentifier, material);

            scene.UseGeneratedAssets(shader, material);
        }
    }



    /// <summary>
    ///     Prefab Mode edits the prefab file, which can't reference the prefab's generated sub-assets. When Prefab Mode
    ///     opens or saves (which reimports the prefab), point the edited scene at the generated assets of the import result.
    ///     <see cref="SdfScene" /> does the same in OnEnable for a stage that is already open, but while Prefab Mode
    ///     opens, OnEnable runs before the stage becomes the current one.
    /// </summary>
    [InitializeOnLoad]
    static class SdfPrefabStageHook {
        static SdfPrefabStageHook() {
            UnityEditor.SceneManagement.PrefabStage.prefabStageOpened += stage => UseGeneratedAssets(stage.prefabContentsRoot, stage.assetPath);
            UnityEditor.SceneManagement.PrefabStage.prefabSaved += root => {
                if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() is { } stage && stage.prefabContentsRoot == root)
                    UseGeneratedAssets(root, stage.assetPath);
            };
        }

        static void UseGeneratedAssets(GameObject root, string prefabPath) {
            if (root.TryGetComponent<SdfScene>(out var scene))
                scene.UseGeneratedAssetsOf(prefabPath);
        }
    }



    /// <summary>
    ///     A custom asset dependency whose value changes whenever the assemblies that generate shaders change, so SDF scene
    ///     prefabs reimport after an update of the generator code, and only then.
    /// </summary>
    [InitializeOnLoad]
    static class SdfGeneratorDependency {
        internal const string Name = "me.tooster.sdf/generator";
        const string SessionKey = Name + "/registered";

        static SdfGeneratorDependency() {
            var hash = new Hash128();
            foreach (var assembly in new[] { typeof(SdfScene).Assembly, typeof(AST.Hlsl.Extensions).Assembly }.Distinct())
                hash.Append(assembly.ManifestModule.ModuleVersionId.ToString());

            AssetDatabase.RegisterCustomDependency(Name, hash);
            // a changed value only reimports the dependent prefabs on the next refresh
            if (SessionState.GetString(SessionKey, "") != hash.ToString()) {
                SessionState.SetString(SessionKey, hash.ToString());
                EditorApplication.delayCall += AssetDatabase.Refresh;
            }
        }

        internal static string[] ShaderLibraryFiles
            => Directory.GetFiles(Path.GetFullPath(SdfPaths.ShaderLibrary.Root), "*.hlsl")
                .Select(file => $"{SdfPaths.ShaderLibrary.Root}/{Path.GetFileName(file)}")
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();
    }
}
