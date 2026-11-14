using System.Linq;
using me.tooster.sdf.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>
    ///     Renders the passes with which the scene view outlines and picks objects, and the object picking that
    ///     <see cref="SdfScenePicking" /> adds. Needs a graphics device, so it's
    ///     skipped when Unity runs with <c>-nographics</c>.
    /// </summary>
    public class ScenePickingTests {
        RenderTestScene world;

        [SetUp]
        public void SetUp() {
            world = new RenderTestScene();
            world.ground.SetActive(false);
        }

        [TearDown]
        public void TearDown() => world?.Dispose();

        bool[] SdfMask() => world.Render().Select(c => c.r + c.g + c.b > 0).ToArray();

        [TestCase("ScenePickingPass")]
        [TestCase("SceneSelectionPass")]
        public void EditorPass_CoversTheSdfInsteadOfTheDomain(string pass) {
            var sdfMask = SdfMask();
            // the values that the editor sets, chosen so that covered pixels are white
            world.material.SetVector("_SelectionID", Vector4.one);
            world.material.SetFloat("_ObjectId", 1);
            var covered = world.RenderPass(pass).Select(c => c.r == 255).ToArray();

            var sdfPixels = sdfMask.Count(c => c);
            Assert.That(sdfPixels, Is.GreaterThan(100), "the SDF isn't visible");
            // both march the same rays; the domain mesh covers many more pixels than the SDF
            var mismatched = sdfMask.Where((c, i) => c != covered[i]).Count();
            Assert.That(mismatched, Is.LessThan(sdfPixels / 10), $"{pass} covers other pixels than the SDF");
        }

        const int FirstPickingIndex = 1;

        /// <summary>Renders the object picking pass and returns the object that each pixel picks.</summary>
        Object[] PickedObjects(System.Func<GameObject, bool> isPickable) {
            var indices = 0;
            var image = world.RenderImmediate(() => indices = SdfScenePicking.Render(new[] { world.scene }, FirstPickingIndex, isPickable));
            Assert.That(indices, Is.GreaterThan(0), "the scene wasn't drawn");
            return image.Select(c => {
                    var index = HandleUtility.DecodeSelectionId(new Vector4(c.r, c.g, c.b, c.a) / 255f);
                    return index == 0 ? null : SdfScenePicking.Resolve(index - FirstPickingIndex);
                })
                .ToArray();
        }

        [Test]
        public void ObjectPicking_FindsTheObjectUnderEachPixel() {
            var sdfMask = SdfMask();
            var picked = PickedObjects(_ => true);

            var sdfPixels = sdfMask.Count(c => c);
            var pickedPixels = picked.Count(o => o != null);
            Assert.That(sdfPixels, Is.GreaterThan(100), "the SDF isn't visible");
            Assert.That(sdfMask.Where((c, i) => c != (picked[i] != null)).Count(), Is.LessThan(sdfPixels / 10), "picking covers other pixels than the SDF");
            foreach (var o in picked.Where(o => o != null).Distinct())
                Assert.That(((GameObject)o).GetComponent<SdfController>(), Is.Not.Null, $"{o} isn't an SDF object");
            // the template has a sphere and a box cut by cylinders
            Assert.That(picked.Where(o => o != null).Distinct().Count(), Is.GreaterThan(1), $"all {pickedPixels} pixels pick one object");
        }

        [Test]
        public void SceneSelectionPass_OutlinesOnlyTheSelectedObject() {
            var picked = PickedObjects(_ => true);
            Assert.That(picked, Has.Some.Not.Null, "nothing is picked");
            var selected = (GameObject)picked.Where(o => o != null).GroupBy(o => o).OrderByDescending(g => g.Count()).First().Key;

            Color32[] outline;
            Shader.SetGlobalFloatArray("_SdfOutlined", SdfSceneOutline.OutlineFlags(world.scene, new[] { selected }));
            Shader.SetGlobalFloat("_SdfOutlineSelected", 1);
            try {
                world.material.SetFloat("_ObjectId", 1);
                outline = world.RenderPass("SceneSelectionPass");
            } finally {
                Shader.SetGlobalFloat("_SdfOutlineSelected", 0);
            }

            var selectedPixels = picked.Select(o => o == selected).ToArray();
            var covered = outline.Select(c => c.r == 255).ToArray();
            var count = selectedPixels.Count(p => p);
            Assert.That(covered.Where((c, i) => c != selectedPixels[i]).Count(), Is.LessThan(count / 10 + 5), "the outline covers other objects");
        }

        [Test]
        public void ObjectPicking_WorksWithMaterialVariants() {
            var variant = new Material(world.material) { parent = world.material };
            try {
                foreach (var renderer in world.root.GetComponentsInChildren<Renderer>(true))
                    renderer.sharedMaterial = variant;

                Assert.That(PickedObjects(_ => true), Has.Some.Not.Null);
            } finally {
                Object.DestroyImmediate(variant);
            }
        }

        [Test]
        public void ObjectPicking_SkipsObjectsThatCantBePicked() {
            var picked = PickedObjects(_ => true);
            Assert.That(picked, Has.Some.Not.Null, "nothing is picked");
            var mostPicked = (GameObject)picked.Where(o => o != null).GroupBy(o => o).OrderByDescending(g => g.Count()).First().Key;

            var withoutIt = PickedObjects(go => go != mostPicked);

            Assert.That(withoutIt, Has.None.EqualTo(mostPicked));
            Assert.That(withoutIt.Count(o => o != null), Is.GreaterThan(0), "no other object is picked");
        }

        [Test]
        public void FocusPass_ShowsTheSelectedObject() {
            var picked = PickedObjects(_ => true);
            var sphere = world.root.GetComponentInChildren<me.tooster.sdf.Primitives.SdfSphereController>(true).GetComponent<SdfController>();
            var id = world.scene.sceneData.controllers[sphere].numericId;

            var focus = world.RenderImmediate(() => Assert.That(SdfSceneFocus.Draw(world.scene, id), "nothing has the focus pass"))
                .Select(c => c.r + c.g + c.b + c.a > 0)
                .ToArray();

            var spherePixels = picked.Count(o => o == sphere.gameObject);
            Assert.That(spherePixels, Is.GreaterThan(10), "the sphere isn't visible");
            var shown = picked.Where((o, i) => o == sphere.gameObject && focus[i]).Count();
            Assert.That(shown, Is.GreaterThan(spherePixels * 95 / 100), "the focus view doesn't show the sphere");
            Assert.That(focus.Count(f => f), Is.LessThan(focus.Length / 2), "the focus view covers more than the sphere");
        }
    }
}
