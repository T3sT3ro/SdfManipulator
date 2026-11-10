#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using me.tooster.sdf.Util;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor {
    /// <summary>
    ///     Migrates SDF scenes from the earlier layouts, a standalone <c>.shader</c> file or a shader and material embedded
    ///     in the prefab file, to the generated sub-assets of <see cref="SdfScenePostprocessor" />, and gives their
    ///     controllers identifiers.
    /// </summary>
    public static class SdfSceneMigration {
        static readonly Regex TargetShaderReference = new(@"targetShader: \{fileID: -?\d+, guid: (\w+), type: \d\}");

        /// <summary>
        ///     For every SDF scene prefab in the project's Assets folder and in the package:
        ///     <list type="number">
        ///         <item>removes the shaders and materials stored in the prefab file, and references to them</item>
        ///         <item>gives every controller without an identifier a new one</item>
        ///         <item>reimports the prefab, which generates its shader and default material</item>
        ///         <item>points every material asset that used an old shader of the scene at the generated shader</item>
        ///         <item>deletes the scene's old standalone <c>.shader</c> file</item>
        ///     </list>
        /// </summary>
        [MenuItem("Tools/SDF/Migrate Scenes")]
        public static void MigrateAll() {
            var prefabs = SdfSceneShaders.FindScenePrefabs("Assets", SdfPaths.PackageRoot);

            // the old shaders of each scene: its standalone target shader and the shaders embedded in its prefab file
            var sceneOfOldShader = new Dictionary<Shader, string>();
            var oldShaderFiles = new HashSet<string>();
            foreach (var prefab in prefabs) {
                if (TargetShaderReference.Match(File.ReadAllText(prefab)) is { Success: true } match
                 && AssetDatabase.GUIDToAssetPath(match.Groups[1].Value) is { } shaderPath
                 && shaderPath.EndsWith(".shader")
                 && AssetDatabase.LoadAssetAtPath<Shader>(shaderPath) is { } targetShader) {
                    sceneOfOldShader[targetShader] = prefab;
                    oldShaderFiles.Add(shaderPath);
                }
                foreach (var embedded in StoredSubAssets(prefab).OfType<Shader>())
                    sceneOfOldShader[embedded] = prefab;
            }

            var materialsToRetarget = AssetDatabase.FindAssets("t:Material", new[] { "Assets" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Distinct()
                .Where(path => path.EndsWith(".mat"))
                .Select(AssetDatabase.LoadAssetAtPath<Material>)
                .Where(material => material != null && material.shader != null && sceneOfOldShader.ContainsKey(material.shader))
                .Select(material => (material, prefab: sceneOfOldShader[material.shader]))
                .ToList();

            foreach (var prefab in prefabs)
                MigratePrefab(prefab);

            foreach (var (material, prefab) in materialsToRetarget) {
                var scene = AssetDatabase.LoadAssetAtPath<GameObject>(prefab).GetComponentInChildren<SdfScene>(true);
                material.shader = scene.generatedShader;
                EditorUtility.SetDirty(material);
                Debug.Log($"Migrated {AssetDatabase.GetAssetPath(material)} to the generated shader of {prefab}");
            }
            AssetDatabase.SaveAssets();

            foreach (var file in oldShaderFiles) {
                AssetDatabase.DeleteAsset(file);
                Debug.Log($"Deleted the old shader {file}");
            }
            Debug.Log($"Migrated {prefabs.Length} SDF scene prefab(s).");
        }

        static void MigratePrefab(string prefab) {
            var stored = StoredSubAssets(prefab);
            var root = PrefabUtility.LoadPrefabContents(prefab);
            try {
                // embedded materials go away; renderers without a material get the generated default material on import
                foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
                    if (renderer.sharedMaterial != null && stored.Contains(renderer.sharedMaterial))
                        renderer.sharedMaterial = null;
                var scene = root.GetComponentInChildren<SdfScene>(true);
                // the loaded contents are in a preview scene, where revalidating assigns missing controller identifiers
                scene.RevalidateScene();
                PrefabUtility.SaveAsPrefabAsset(root, prefab);
            } finally {
                PrefabUtility.UnloadPrefabContents(root);
            }

            foreach (var subAsset in StoredSubAssets(prefab))
                AssetDatabase.RemoveObjectFromAsset(subAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(prefab, ImportAssetOptions.ForceUpdate);
        }

        static Object[] StoredSubAssets(string prefab)
            => AssetDatabase.LoadAllAssetRepresentationsAtPath(prefab)
                .Where(subAsset => subAsset is Shader or Material && SdfScene.IsStoredInFile(subAsset, prefab))
                .ToArray();
    }
}
