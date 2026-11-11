using System.IO;
using System.Linq;
using me.tooster.sdf.Editor;
using me.tooster.sdf.Operators;
using me.tooster.sdf.Primitives;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>
    ///     Checks that importing an SDF scene prefab generates its shader and default material as sub-assets of the
    ///     prefab, outside of the prefab file.
    /// </summary>
    public class SceneSubAssetTests {
        const string Folder = "Assets/SdfSubAssetTests";
        string prefabPath;

        [SetUp]
        public void CreateScene() {
            if (!AssetDatabase.IsValidFolder(Folder))
                AssetDatabase.CreateFolder("Assets", "SdfSubAssetTests");
            prefabPath = AssetDatabase.GetAssetPath(SdfSceneCreation.CreateScene($"{Folder}/sub-asset scene.prefab"));
        }

        [TearDown]
        public void DeleteScene() {
            StageUtility.GoToMainStage();
            AssetDatabase.DeleteAsset(Folder);
        }

        [Test]
        public void Import_GeneratesShaderAndDefaultMaterial() {
            var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(prefabPath);
            var shader = subAssets.OfType<Shader>().Single();
            var material = subAssets.OfType<Material>().Single();
            Assert.That(material.shader, Is.EqualTo(shader));
            Assert.That(ShaderUtil.ShaderHasError(shader), Is.False, "the generated shader has errors");

            var scene = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath).GetComponentInChildren<SdfScene>(true);
            Assert.That(scene.generatedShader, Is.EqualTo(shader));
            Assert.That(scene.generatedMaterial, Is.EqualTo(material));
            Assert.That(scene.controlledMaterial, Is.EqualTo(material));
            Assert.That(scene.GetComponentsInChildren<Renderer>(true).Select(r => r.sharedMaterial), Has.All.EqualTo(material));
        }

        [Test]
        public void Import_KeepsPrefabFileFreeOfGeneratedAssets() {
            var text = File.ReadAllText(prefabPath);
            StringAssert.DoesNotContain("--- !u!48 ", text, "the prefab file contains a shader");
            StringAssert.DoesNotContain("--- !u!21 ", text, "the prefab file contains a material");
            Assert.That(Directory.GetFiles(Folder).Where(f => !f.EndsWith(".meta")).ToArray(), Has.Length.EqualTo(1), "the scene created more files");
        }

        [Test]
        public void Import_KeepsMaterialAssetThatUsesGeneratedShader() {
            var generatedShader = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath).GetComponentInChildren<SdfScene>(true).generatedShader;
            var custom = new Material(generatedShader);
            AssetDatabase.CreateAsset(custom, $"{Folder}/custom.mat");

            var root = PrefabUtility.LoadPrefabContents(prefabPath);
            try {
                foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
                    renderer.sharedMaterial = custom;
                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            } finally {
                PrefabUtility.UnloadPrefabContents(root);
            }

            var scene = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath).GetComponentInChildren<SdfScene>(true);
            Assert.That(scene.controlledMaterial, Is.EqualTo(custom));
            Assert.That(scene.GetComponentsInChildren<Renderer>(true).Select(r => r.sharedMaterial), Has.All.EqualTo(custom));
            Assert.That(custom.shader, Is.EqualTo(scene.generatedShader), "the custom material lost the generated shader");
        }

        [Test]
        public void Import_IgnoresSceneInNestedPrefab() {
            var scenePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var outerPath = $"{Folder}/outer.prefab";
            var outer = new GameObject("outer");
            try {
                PrefabUtility.InstantiatePrefab(scenePrefab, outer.transform);
                PrefabUtility.SaveAsPrefabAsset(outer, outerPath);
            } finally {
                Object.DestroyImmediate(outer);
            }

            Assert.That(AssetDatabase.LoadAllAssetRepresentationsAtPath(outerPath).OfType<Shader>(), Is.Empty, "the outer prefab generated a shader");
            var nested = AssetDatabase.LoadAssetAtPath<GameObject>(outerPath).GetComponentInChildren<SdfScene>(true);
            Assert.That(nested.controlledMaterial, Is.EqualTo(scenePrefab.GetComponent<SdfScene>().controlledMaterial));
        }

        [Test]
        public void Reimport_KeepsGeneratedObjects() {
            var before = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath).GetComponentInChildren<SdfScene>(true);
            var (shader, material) = (before.generatedShader, before.generatedMaterial);

            AssetDatabase.ImportAsset(prefabPath, ImportAssetOptions.ForceUpdate);

            // the managed wrappers may change, but existing references must still point to live objects that equal the new ones
            var after = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath).GetComponentInChildren<SdfScene>(true);
            Assert.That(shader != null, "references to the old shader point to a destroyed object");
            Assert.That(material != null, "references to the old material point to a destroyed object");
            Assert.That(after.generatedShader == shader, "the reimported shader is a different object");
            Assert.That(after.generatedMaterial == material, "the reimported material is a different object");
        }

        [Test]
        public void PrefabMode_UsesGeneratedAssetsAndUpdatesShaderBeforeSaving() {
            var stage = PrefabStageUtility.OpenPrefab(prefabPath);
            var scene = stage.prefabContentsRoot.GetComponentInChildren<SdfScene>(true);
            var imported = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath).GetComponentInChildren<SdfScene>(true);
            Assert.That(scene.generatedShader, Is.EqualTo(imported.generatedShader));
            Assert.That(scene.controlledMaterial, Is.EqualTo(imported.generatedMaterial));
            Assert.That(scene.GetComponentsInChildren<Renderer>(true).Select(r => r.sharedMaterial), Has.All.EqualTo(imported.generatedMaterial));

            var propertiesBefore = ShaderUtil.GetPropertyCount(scene.generatedShader);
            var combine = scene.GetComponentInChildren<SdfCombineController>(true);
            var added = new GameObject("added sphere");
            added.transform.SetParent(combine.transform, false);
            var sdf = added.AddComponent<SdfController>();
            sdf.sdfModifiers = new Controller[] { added.AddComponent<SdfSphereController>() };
            combine.children = combine.children.Append(sdf).ToArray();

            scene.RevalidateScene();
            scene.GenerateSceneAssets();

            Assert.That(ShaderUtil.GetPropertyCount(scene.generatedShader), Is.GreaterThan(propertiesBefore), "the shader didn't get the new sphere's uniform");
            Assert.That(ShaderUtil.ShaderHasError(scene.generatedShader), Is.False);
        }
    }
}
