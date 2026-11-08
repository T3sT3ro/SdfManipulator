using System.Linq;
using me.tooster.sdf.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>
    ///     Creates a scene the way <i>Assets &gt; Create &gt; SDF &gt; Scene Asset</i> does and checks that it can
    ///     regenerate its own shader.
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
        public void CreateScene_WiresOwnShaderAndMaterialFiles() {
            var prefabPath = $"{Folder}/new scene.prefab";
            var prefab = SdfSceneCreation.CreateScene(prefabPath);
            var scene = prefab.GetComponentInChildren<SdfScene>(true);

            Assert.That(AssetDatabase.GetAssetPath(scene.targetShader), Is.EqualTo($"{Folder}/new scene.shader"));
            Assert.That(scene.TargetShaderPath, Is.Not.Null, "the target shader can't be regenerated");
            Assert.That(AssetDatabase.GetAssetPath(scene.controlledMaterial), Is.EqualTo($"{Folder}/new scene.mat"));
            Assert.That(scene.controlledMaterial.shader, Is.EqualTo(scene.targetShader));

            var renderers = prefab.GetComponentsInChildren<Renderer>(true);
            Assert.That(renderers, Is.Not.Empty);
            Assert.That(renderers.Select(r => r.sharedMaterial), Has.All.EqualTo(scene.controlledMaterial));

            Assert.That(AssetDatabase.LoadAllAssetRepresentationsAtPath(prefabPath), Is.Empty, "the prefab has sub-assets");
        }
    }
}
