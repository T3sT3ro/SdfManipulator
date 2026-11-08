#nullable enable
using System.IO;
using me.tooster.sdf.Util;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor {
    /// <summary>
    ///     Creates new SDF scene assets from the package's scene template.
    /// </summary>
    public static class SdfSceneCreation {
        /// <summary>
        ///     Creates an SDF scene prefab at <paramref name="prefabPath" /> from the template, together with its own shader
        ///     and material files in the same folder: <c>Name.shader</c> and <c>Name.mat</c>. The scene's
        ///     <see cref="SdfScene.targetShader" />, <see cref="SdfScene.controlledMaterial" /> and the renderers that
        ///     used the template's material point to the new files, so the scene can regenerate its shader.
        /// </summary>
        /// <returns>the created prefab asset</returns>
        public static GameObject CreateScene(string prefabPath) {
            prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
            var directory = Path.GetDirectoryName(prefabPath)!.Replace('\\', '/');
            var sceneName = Path.GetFileNameWithoutExtension(prefabPath);
            var shaderPath = AssetDatabase.GenerateUniqueAssetPath($"{directory}/{sceneName}.shader");
            var materialPath = AssetDatabase.GenerateUniqueAssetPath($"{directory}/{sceneName}.mat");

            var root = PrefabUtility.LoadPrefabContents(SdfPaths.Templates.HelloSdfPrefab);
            try {
                root.name = sceneName;
                var scene = root.GetComponentInChildren<SdfScene>(true);
                var templateMaterial = scene.controlledMaterial;

                scene.RevalidateScene();
                File.WriteAllText(shaderPath, scene.AssembleShaderSource());
                AssetDatabase.ImportAsset(shaderPath);
                var shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);

                var material = new Material(shader);
                if (templateMaterial != null)
                    material.CopyPropertiesFromMaterial(templateMaterial);
                AssetDatabase.CreateAsset(material, materialPath);

                // The template's material and shader are sub-assets of the template. Nothing may reference them anymore,
                // otherwise the new prefab would depend on the package's template file.
                foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
                    if (renderer.sharedMaterial == templateMaterial)
                        renderer.sharedMaterial = material;
                scene.targetShader = shader;
                scene.controlledMaterial = material;

                return PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            } finally {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }
}
