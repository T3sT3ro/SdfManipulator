using System.IO;
using System.Linq;
using me.tooster.sdf.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>
    ///     Creates a scene the way <i>Assets &gt; Create &gt; SDF &gt; Scene Asset</i> does.
    /// </summary>
    public class SceneCreationTests {
        const string Folder = "Assets/SdfSceneCreationTests";

        [SetUp]
        public void CreateFolder() {
            if (!AssetDatabase.IsValidFolder(Folder))
                AssetDatabase.CreateFolder("Assets", "SdfSceneCreationTests");
        }

        [TearDown]
        public void DeleteFolder() => AssetDatabase.DeleteAsset(Folder);

        [Test]
        public void CreateScene_UsesItsOwnGeneratedAssets() {
            var prefab = SdfSceneCreation.CreateScene($"{Folder}/new scene.prefab");
            var path = AssetDatabase.GetAssetPath(prefab);
            var scene = prefab.GetComponentInChildren<SdfScene>(true);

            Assert.That(prefab.name, Is.EqualTo("new scene"));
            Assert.That(AssetDatabase.GetAssetPath(scene.generatedShader), Is.EqualTo(path), "the shader isn't a sub-asset of the scene");
            Assert.That(AssetDatabase.GetAssetPath(scene.generatedMaterial), Is.EqualTo(path), "the material isn't a sub-asset of the scene");
            Assert.That(scene.GetComponentsInChildren<Renderer>(true).Select(r => r.sharedMaterial), Has.All.EqualTo(scene.generatedMaterial));
            Assert.That(Directory.GetFiles(Folder).Where(f => !f.EndsWith(".meta")), Is.EquivalentTo(new[] { $"{Folder}/new scene.prefab" }));
        }
    }
}
