#nullable enable
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor {
    /// <summary>
    ///     Regenerates the shaders of SDF scene prefabs without opening them in the prefab stage, for example after a
    ///     package update changed the generated code.
    /// </summary>
    public static class SdfSceneShaders {
        /// <summary>
        ///     Regenerates the target shader of every SDF scene prefab in the project's Assets folder.
        /// </summary>
        [MenuItem("Tools/SDF/Regenerate All Scene Shaders")]
        public static void RegenerateAll() {
            var prefabPaths = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponentInChildren<SdfScene>(true) != null)
                .OrderBy(path => path)
                .ToList();

            var written = 0;
            foreach (var path in prefabPaths)
                if (Regenerate(path))
                    written++;

            Debug.Log($"Regenerated SDF scene shaders: {written} of {prefabPaths.Count} scene prefab(s) changed.");
        }

        /// <summary>
        ///     Regenerates the target shader of the SDF scene in the prefab at <paramref name="prefabPath" />. The prefab
        ///     itself isn't modified.
        /// </summary>
        /// <returns>true if the shader file changed</returns>
        public static bool Regenerate(string prefabPath) {
            var root = PrefabUtility.LoadPrefabContents(prefabPath);
            try {
                var scene = root.GetComponentInChildren<SdfScene>(true);
                if (scene == null)
                    return false;

                if (scene.TargetShaderPath == null) {
                    Debug.LogWarning($"Skipped {prefabPath}: its SDF scene has no standalone .shader file as the target shader.");
                    return false;
                }

                scene.RevalidateScene();
                var writtenPath = scene.WriteTargetShader();
                if (writtenPath == null)
                    return false;

                AssetDatabase.ImportAsset(writtenPath);
                return true;
            } finally {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }
}
