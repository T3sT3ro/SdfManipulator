using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>
    ///     Renders the passes with which the scene view outlines and picks objects. Needs a graphics device, so it's
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
    }
}
