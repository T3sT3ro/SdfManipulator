using System.Linq;
using me.tooster.sdf.Editor;
using me.tooster.sdf.Primitives;
using me.tooster.sdf.Util;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>
    ///     Uniform and function names come from identifiers that are assigned once, so renaming or moving objects doesn't
    ///     rename them.
    /// </summary>
    public class ControllerIdentifierTests {
        const string Folder = "Assets/SdfControllerIdentifierTests";
        GameObject   root;
        SdfScene     scene;

        [SetUp]
        public void LoadTemplate() {
            root = PrefabUtility.LoadPrefabContents(SdfPaths.Templates.HelloSdfPrefab);
            scene = root.GetComponentInChildren<SdfScene>(true);
        }

        [TearDown]
        public void Unload() {
            if (root != null)
                PrefabUtility.UnloadPrefabContents(root);
            AssetDatabase.DeleteAsset(Folder);
        }

        string IdentifierOf(Controller controller) => scene.sceneData.controllers[controller].identifier;

        [Test]
        public void Identifiers_StayWhenObjectsAreRenamedAndReordered() {
            scene.RevalidateScene();
            var before = root.GetComponentsInChildren<Controller>(true).ToDictionary(c => c, IdentifierOf);

            var sphere = root.GetComponentInChildren<SdfSphereController>(true);
            sphere.name = "renamed";
            sphere.transform.SetAsFirstSibling();
            scene.RevalidateScene();

            Assert.That(before.Keys.ToDictionary(c => c, IdentifierOf), Is.EqualTo(before));
        }

        [Test]
        public void Identifiers_AreUniqueValidHlslNames() {
            scene.RevalidateScene();
            var names = scene.sceneData.Properties.Select(p => p.identifier)
                .Concat(scene.sceneData.controllers.Values.Select(c => c.identifier))
                .ToList();

            Assert.That(names, Is.Unique);
            // names that contain "__" are reserved in HLSL
            Assert.That(names, Has.All.Matches<string>(n => System.Text.RegularExpressions.Regex.IsMatch(n, "^SDF_[A-Za-z0-9]+(_[A-Za-z0-9]+)?$")));
        }

        [Test]
        public void DuplicatedController_GetsItsOwnIdentifier() {
            var sphere = root.GetComponentInChildren<SdfSphereController>(true);
            scene.RevalidateScene();
            var copy = Object.Instantiate(sphere.gameObject, sphere.transform.parent).GetComponent<SdfSphereController>();
            Assert.That(copy.Id, Is.EqualTo(sphere.Id), "the copy should start with the same serialized identifier");

            scene.RevalidateScene();

            Assert.That(copy.Id, Is.Not.EqualTo(sphere.Id));
            Assert.That(IdentifierOf(copy), Is.Not.EqualTo(IdentifierOf(sphere)));
        }

        [Test]
        public void Import_UsesTheIdentifiersSavedInThePrefab() {
            if (!AssetDatabase.IsValidFolder(Folder))
                AssetDatabase.CreateFolder("Assets", "SdfControllerIdentifierTests");
            var prefab = SdfSceneCreation.CreateScene($"{Folder}/identifier scene.prefab");
            var imported = prefab.GetComponentInChildren<SdfScene>(true);
            var controllers = prefab.GetComponentsInChildren<Controller>(true);

            Assert.That(controllers.Select(c => c.Id), Has.None.Empty, "a new scene saves its controllers' identifiers");
            Assert.That(controllers.Select(c => imported.sceneData.controllers[c].identifier), Is.EqualTo(controllers.Select(c => $"SDF_{c.Id}")));
        }
    }
}
