using System.Linq;
using me.tooster.sdf.Primitives;
using me.tooster.sdf.Util;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>
    ///     Checks how controllers notify their scenes about structural changes.
    /// </summary>
    public class ControllerEventsTests {
        GameObject templateRoot;

        [SetUp]
        public void LoadTemplate() => templateRoot = PrefabUtility.LoadPrefabContents(SdfPaths.Templates.HelloSdfPrefab);

        [TearDown]
        public void Unload() {
            if (templateRoot != null)
                PrefabUtility.UnloadPrefabContents(templateRoot);
        }

        [Test]
        public void MovingObjectToAnotherScene_MarksBothScenes() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            var otherRoot = new GameObject("other scene");
            SceneManager.MoveGameObjectToScene(otherRoot, templateRoot.scene);
            var otherScene = otherRoot.AddComponent<SdfScene>();
            scene.RevalidateScene();
            otherScene.RevalidateScene();
            scene.RequiresRegeneration = false;
            otherScene.RequiresRegeneration = false;

            // An object with several controllers, e.g. a transform, a primitive and the SDF that uses both. Re-parenting
            // one used to subscribe its SDF controller to its own event, and the next notification overflowed the stack.
            var moved = templateRoot.GetComponentsInChildren<SdfController>(true)
                .First(sdf => sdf.GetComponents<Controller>().Length > 1)
                .transform;
            moved.SetParent(otherRoot.transform, false);

            Assert.That(scene.RequiresRegeneration, Is.True, "the scene that the object left isn't marked");
            Assert.That(otherScene.RequiresRegeneration, Is.True, "the scene that the object joined isn't marked");
        }

        [Test]
        public void ToString_OutsideScene_DoesNotThrow() {
            var loose = new GameObject("loose sphere");
            try {
                var sphere = loose.AddComponent<SdfSphereController>();
                Assert.That(sphere.ToString(), Does.Contain("loose sphere"));
            } finally {
                Object.DestroyImmediate(loose);
            }
        }
    }
}
