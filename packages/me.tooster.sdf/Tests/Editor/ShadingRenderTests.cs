using System.Linq;
using me.tooster.sdf.Operators;
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
        public void SdfScene_ShadesObjectsWithTheInheritedAlbedo() {
            world.ground.SetActive(false);
            // the objects without a material use the material on the root combination
            var material = world.root.GetComponentInChildren<SdfCombineController>(true).gameObject.AddComponent<SdfMaterialController>();
            material.Albedo = Color.red;
            world.Regenerate();

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

        [Test]
        public void ClipToDomainBox_KeepsTheImageAndTakesFewerSteps() {
            world.ground.SetActive(false);
            var settings = world.scene.rendering;
            Color32[] Render(bool clip, DrawMode mode) {
                settings.clipToDomainBox = clip;
                settings.drawMode = mode;
                settings.maxSteps = 50;
                world.scene.UpdateShaderUniforms(forceUpdateAll: true);
                return world.Render();
            }

            var full = Render(false, DrawMode.Material);
            var clipped = Render(true, DrawMode.Material);
            var covered = full.Count(c => c.r + c.g + c.b > 0);
            Assert.That(covered, Is.GreaterThan(100), "the SDF isn't visible");
            var changed = full.Where((c, i) => Mathf.Abs(c.r - clipped[i].r) + Mathf.Abs(c.g - clipped[i].g) + Mathf.Abs(c.b - clipped[i].b) > 30).Count();
            Assert.That(changed, Is.LessThan(covered / 50), "clipping changes the image of a scene inside its domain");

            // the steps view shows the number of steps in red
            float MeanSteps(Color32[] image) => (float)image.Where(c => c.r + c.g + c.b > 0).Average(c => c.r);
            Assert.That(MeanSteps(Render(true, DrawMode.Steps)), Is.LessThan(MeanSteps(Render(false, DrawMode.Steps))));
        }

        // the configurations of #104: both projections and the camera inside the domain, with and without clipping
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void SdfScene_RendersForTheCameraConfiguration(bool insideDomain, bool clipToDomainBox) {
            world.ground.SetActive(false);
            world.scene.rendering.clipToDomainBox = clipToDomainBox;
            world.scene.UpdateShaderUniforms(forceUpdateAll: true);
            var perspective = world.Render().Count(c => c.r + c.g + c.b > 0);

            world.camera.orthographic = true;
            world.camera.orthographicSize = 2;
            var orthographic = world.Render().Count(c => c.r + c.g + c.b > 0);

            Assert.That(perspective, Is.GreaterThan(100), "the SDF isn't visible with a perspective camera");
            Assert.That(orthographic, Is.GreaterThan(100), "the SDF isn't visible with an orthographic camera");

            if (!insideDomain)
                return;
            // the domain is a cube 3 units wide around the origin; look at the sphere from inside it, through no face
            world.camera.orthographic = false;
            world.camera.transform.position = new Vector3(0, 0, -1.3f);
            world.camera.transform.rotation = Quaternion.identity;
            world.camera.nearClipPlane = 0.05f;
            var inside = world.Render().Count(c => c.r + c.g + c.b > 0);
            Assert.That(inside, Is.GreaterThan(RenderTestScene.Size * RenderTestScene.Size / 4), "the SDF isn't visible from inside the domain");
        }

        // #13: a depth prepass over back faces cut off the rays of overlapping domains; rays now start on the near plane
        [Test]
        public void OverlappingDomains_ShowBothScenes() {
            world.ground.SetActive(false);
            var copy = Object.Instantiate(world.root);
            world.Add(copy);
            try {
                copy.transform.position = new Vector3(1, 0, 0.5f);
                var copyScene = copy.GetComponentInChildren<SdfScene>(true);
                copyScene.RevalidateScene();
                copyScene.UpdateShaderUniforms(forceUpdateAll: true);
                var originalRenderers = world.root.GetComponentsInChildren<Renderer>(true);
                var copyRenderers = copy.GetComponentsInChildren<Renderer>(true);
                bool[] Coverage(bool original, bool copied) {
                    foreach (var r in originalRenderers) r.enabled = original;
                    foreach (var r in copyRenderers) r.enabled = copied;
                    return world.Render().Select(c => c.r + c.g + c.b > 0).ToArray();
                }

                var onlyOriginal = Coverage(true, false);
                var onlyCopy = Coverage(false, true);
                var both = Coverage(true, true);

                var union = onlyOriginal.Where((c, i) => c || onlyCopy[i]).Count();
                var mismatched = both.Where((c, i) => c != (onlyOriginal[i] || onlyCopy[i])).Count();
                Assert.That(union, Is.GreaterThan(onlyOriginal.Count(c => c)), "the copy doesn't show beside the original");
                Assert.That(mismatched, Is.LessThan(union / 50), "overlapping domains hide parts of each other's scene");
            } finally {
                Object.DestroyImmediate(copy);
            }
        }

        static int DarkerPixels(Color32[] image, Color32[] reference, bool[] mask = null)
            => image.Where((c, i) => (mask == null || mask[i]) && Luminance(reference[i]) - Luminance(c) > 0.1f).Count();

        static float Luminance(Color32 c) => (0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b) / 255f;
    }
}
