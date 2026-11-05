using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace SdfManipulator.DevTools {
    /// <summary>
    ///     Compiles the default variant of every vertex and fragment program in the dev project's and the SDF package's
    ///     shaders (including those SDF scene prefabs carry as sub-assets) and reports errors. Compiles explicitly for
    ///     Vulkan, so it works in batchmode with -nographics, where Unity otherwise compiles variants lazily.
    ///     Batchmode:
    ///     <c>Unity -batchmode -quit -projectPath projects/SdfManipulator -executeMethod SdfManipulator.DevTools.ShaderErrorsCheck.Run</c>
    /// </summary>
    static class ShaderErrorsCheck {
        static readonly ShaderType[] Stages = { ShaderType.Vertex, ShaderType.Fragment };

        [MenuItem("Tools/SDF Dev/Check Shader Errors")]
        public static void Run() {
            var paths = AssetDatabase.FindAssets("t:Shader", new[] { "Assets", "Packages/me.tooster.sdf" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Distinct()
                .OrderBy(path => path)
                .ToList();

            int shaders = 0, failed = 0;
            foreach (var path in paths) {
                foreach (var shader in AssetDatabase.LoadAllAssetsAtPath(path).OfType<Shader>()) {
                    shaders++;
                    var errors = Errors(shader).Distinct().ToList();
                    if (errors.Count == 0)
                        continue;
                    failed++;
                    foreach (var error in errors)
                        Debug.LogError($"[ShaderErrorsCheck] {path} ({shader.name}): {error}");
                }
            }

            Debug.Log($"[ShaderErrorsCheck] checked {shaders} shader(s) in {paths.Count} asset(s), {failed} with errors");
            if (Application.isBatchMode)
                EditorApplication.Exit(failed == 0 ? 0 : 1);
        }

        static IEnumerable<string> Errors(Shader shader) {
            foreach (var message in ShaderUtil.GetShaderMessages(shader))
                if (message.severity == ShaderCompilerMessageSeverity.Error)
                    yield return $"line {message.line}: {message.message}";

            var data = ShaderUtil.GetShaderData(shader);
            for (var s = 0; s < data.SubshaderCount; s++) {
                var subshader = data.GetSubshader(s);
                for (var p = 0; p < subshader.PassCount; p++) {
                    var pass = subshader.GetPass(p);
                    foreach (var stage in Stages) {
                        if (!pass.HasShaderStage(stage))
                            continue;
                        var result = pass.CompileVariant(stage, new string[0], ShaderCompilerPlatform.Vulkan, BuildTarget.StandaloneLinux64);
                        foreach (var message in result.Messages)
                            if (message.severity == ShaderCompilerMessageSeverity.Error)
                                yield return $"subshader {s} pass {p} {stage}: {message.file}:{message.line}: {message.message}";
                    }
                }
            }
        }
    }
}
