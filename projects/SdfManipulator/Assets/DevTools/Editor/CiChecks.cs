using UnityEditor;
using UnityEngine;

namespace SdfManipulator.DevTools {
    /// <summary>
    ///     Runs every editor check in one Unity session, for CI. Exits with 0 only if all of them pass.
    ///     <c>Unity -batchmode -quit -projectPath projects/SdfManipulator -executeMethod SdfManipulator.DevTools.CiChecks.Run</c>
    /// </summary>
    static class CiChecks {
        public static void Run() {
            var playerScripts = PlayerScriptsCheck.Check();
            var shaders = ShaderErrorsCheck.Check();
            Debug.Log($"[CiChecks] player scripts: {(playerScripts ? "OK" : "FAILED")}, shaders: {(shaders ? "OK" : "FAILED")}");
            EditorApplication.Exit(playerScripts && shaders ? 0 : 1);
        }
    }
}
