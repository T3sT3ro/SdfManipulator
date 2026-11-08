using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SdfManipulator.DevTools {
    /// <summary>
    ///     Builds a Linux player of the Showcase scene, to check that SDF scenes work outside the editor. Run the player
    ///     headless and look for exceptions in its log:
    ///     <c>Builds/PlayerBuildCheck/SdfShowcase.x86_64 -batchmode -nographics -logFile -</c>
    ///     Batchmode:
    ///     <c>Unity -batchmode -quit -projectPath projects/SdfManipulator -executeMethod SdfManipulator.DevTools.PlayerBuildCheck.Build</c>
    /// </summary>
    static class PlayerBuildCheck {
        const string Scene      = "Assets/Examples/Showcase.unity";
        const string OutputPath = "Builds/PlayerBuildCheck/SdfShowcase.x86_64";

        [MenuItem("Tools/SDF Dev/Build Showcase Player")]
        public static void Build() {
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { Scene },
                locationPathName = OutputPath,
                target = BuildTarget.StandaloneLinux64,
                options = BuildOptions.Development,
            });
            var ok = report.summary.result == BuildResult.Succeeded;
            Debug.Log($"[PlayerBuildCheck] {report.summary.result}: {OutputPath}, {report.summary.totalErrors} error(s)");
            if (Application.isBatchMode)
                EditorApplication.Exit(ok ? 0 : 1);
        }
    }
}
