#nullable enable
using me.tooster.sdf.Util;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor {
    /// <summary>
    ///     Creates new SDF scene assets from the package's scene template.
    /// </summary>
    public static class SdfSceneCreation {
        /// <summary>
        ///     Creates an SDF scene prefab at <paramref name="prefabPath" /> from the template. Importing the prefab
        ///     generates its shader and default material as sub-assets (see <see cref="SdfScenePostprocessor" />), so no
        ///     other files are created.
        /// </summary>
        /// <returns>the created prefab asset</returns>
        public static GameObject CreateScene(string prefabPath) {
            prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
            var root = PrefabUtility.LoadPrefabContents(SdfPaths.Templates.HelloSdfPrefab);
            try {
                root.name = System.IO.Path.GetFileNameWithoutExtension(prefabPath);
                var scene = root.GetComponentInChildren<SdfScene>(true);
                // anything the template references belongs to the template; the import assigns the new scene's own assets
                scene.controlledMaterial = null;
                foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
                    renderer.sharedMaterial = null;
                // the new scene's controllers get their own identifiers, which are saved with it
                foreach (var controller in root.GetComponentsInChildren<Controller>(true))
                    controller.AssignNewId();

                return PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            } finally {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }
}
