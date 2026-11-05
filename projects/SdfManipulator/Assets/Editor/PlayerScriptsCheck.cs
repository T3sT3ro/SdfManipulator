using System.Linq;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEditor.Compilation;
using UnityEngine;

namespace SdfManipulator.DevTools {
    /// <summary>
    ///     Compiles the player scripts without building a player, to catch editor-only API used in runtime assemblies.
    ///     Batchmode:
    ///     <c>Unity -batchmode -quit -projectPath projects/SdfManipulator -executeMethod SdfManipulator.DevTools.PlayerScriptsCheck.Run</c>
    /// </summary>
    static class PlayerScriptsCheck {
        const string OutputDirectory = "Temp/PlayerScriptsCheck";

        [MenuItem("Tools/SDF Dev/Compile Player Scripts")]
        public static void Run() {
            var settings = new ScriptCompilationSettings
            {
                target = BuildTarget.StandaloneLinux64,
                group = BuildTargetGroup.Standalone,
            };
            var result = PlayerBuildInterface.CompilePlayerScripts(settings, OutputDirectory);

            var expected = CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies)
                .Select(assembly => assembly.name + ".dll")
                .Where(name => name.StartsWith("me.tooster.sdf"))
                .ToList();
            var missing = expected.Except(result.assemblies).ToList();

            if (missing.Count == 0)
                Debug.Log($"[PlayerScriptsCheck] OK: compiled {string.Join(", ", expected)}");
            else
                Debug.LogError($"[PlayerScriptsCheck] FAILED: not compiled for the player: {string.Join(", ", missing)}");

            if (Application.isBatchMode)
                EditorApplication.Exit(missing.Count == 0 ? 0 : 1);
        }
    }
}
