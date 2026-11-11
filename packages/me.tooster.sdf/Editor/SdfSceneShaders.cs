#nullable enable
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor {
    /// <summary>
    ///     Regenerates the shaders of SDF scene prefabs without opening them in Prefab Mode. The shaders are generated
    ///     when a prefab is imported, so regenerating means reimporting the prefab.
    /// </summary>
    public static class SdfSceneShaders {
        /// <summary>
        ///     Reimports every SDF scene prefab in the project's Assets folder.
        /// </summary>
        [MenuItem("Tools/SDF/Regenerate All Scene Shaders")]
        public static void RegenerateAll() {
            var prefabPaths = FindScenePrefabs("Assets");
            foreach (var path in prefabPaths)
                Regenerate(path);
            Debug.Log($"Regenerated the shaders of {prefabPaths.Length} SDF scene prefab(s).");
        }

        /// <summary>
        ///     Reimports the SDF scene prefab at <paramref name="prefabPath" />, which regenerates its shader and default
        ///     material.
        /// </summary>
        public static void Regenerate(string prefabPath) => AssetDatabase.ImportAsset(prefabPath, ImportAssetOptions.ForceUpdate);

        internal static string[] FindScenePrefabs(params string[] folders)
            => AssetDatabase.FindAssets("t:Prefab", folders)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponentInChildren<SdfScene>(true) != null)
                .Distinct()
                .OrderBy(path => path)
                .ToArray();
    }
}
