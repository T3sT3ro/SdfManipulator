using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>
    ///     Renders the template scene above a ground plane with a directional light, and compares images with and without
    ///     shadow casters. Needs a graphics device, so it's skipped when Unity runs with <c>-nographics</c>.
    /// </summary>
    public class ShadingRenderTests {
        RenderTestScene world;

        [SetUp]
        public void SetUp() => world = new RenderTestScene();

        [TearDown]
        public void TearDown() => world?.Dispose();

        [Test]
        public void SdfScene_CastsShadowOnOtherGeometry() {
            var sdfRenderers = world.root.GetComponentsInChildren<Renderer>(true);

            foreach (var r in sdfRenderers)
                r.shadowCastingMode = ShadowCastingMode.Off;
            var withoutShadow = world.Render();
            foreach (var r in sdfRenderers)
                r.shadowCastingMode = ShadowCastingMode.On;
            var withShadow = world.Render();

            Assert.That(DarkerPixels(withShadow, withoutShadow), Is.GreaterThan(100), "the SDF casts no shadow on the ground");
        }

        [Test]
        public void SdfScene_ReceivesShadowOfOtherGeometry() {
            world.ground.SetActive(false);
            var sdfMask = world.Render().Select(c => c.r + c.g + c.b > 0).ToArray();
            Assert.That(sdfMask.Count(covered => covered), Is.GreaterThan(100), "the SDF isn't visible");

            // a slab between the light and the scene
            var occluder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            world.Add(occluder);
            occluder.transform.position = -world.light.transform.forward * 3.5f;
            occluder.transform.rotation = world.light.transform.rotation;
            occluder.transform.localScale = new Vector3(3, 3, 0.2f);
            try {
                var occluderRenderer = occluder.GetComponent<Renderer>();
                occluderRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                var shadowed = world.Render();
                occluderRenderer.enabled = false;
                var lit = world.Render();

                // surfaces that face away from the light are dark either way
                var darkerSdfPixels = DarkerPixels(shadowed, lit, sdfMask);
                Assert.That(darkerSdfPixels, Is.GreaterThan(sdfMask.Count(covered => covered) / 4), "the SDF isn't shadowed");
            } finally {
                Object.DestroyImmediate(occluder);
            }
        }

        [Test]
        public void SdfScene_ShadesObjectsWithTheirAlbedo() {
            world.ground.SetActive(false);
            foreach (var sdf in world.root.GetComponentsInChildren<SdfController>(true))
                sdf.Albedo = Color.red;
            world.scene.UpdateShaderUniforms(forceUpdateAll: true);

            var covered = world.Render().Where(c => c.r + c.g + c.b > 0).ToList();
            Assert.That(covered.Count, Is.GreaterThan(100), "the SDF isn't visible");
            // ambient light from the sky adds some green and blue
            Assert.That(covered.Average(c => c.r), Is.GreaterThan(2 * covered.Average(c => c.g)));
            Assert.That(covered.Average(c => c.r), Is.GreaterThan(2 * covered.Average(c => c.b)));
        }

        [Test]
        public void DrawMode_SwitchesWithoutAnotherVariant() {
            world.ground.SetActive(false);
            var shaded = world.Render();
            world.scene.rendering.drawMode = DrawMode.Normal;
            world.scene.UpdateShaderUniforms(forceUpdateAll: true);
            var normals = world.Render();

            var changed = shaded.Where((c, i) => Mathf.Abs(c.r - normals[i].r) + Mathf.Abs(c.g - normals[i].g) + Mathf.Abs(c.b - normals[i].b) > 30).Count();
            Assert.That(changed, Is.GreaterThan(100), "the draw mode doesn't change the image");
        }

        static int DarkerPixels(Color32[] image, Color32[] reference, bool[] mask = null)
            => image.Where((c, i) => (mask == null || mask[i]) && Luminance(reference[i]) - Luminance(c) > 0.1f).Count();

        static float Luminance(Color32 c) => (0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b) / 255f;
    }
}
