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

        [Test]
        public void Subtraction_RemovesTheOtherChildrenFromTheFirst() {
            world.ground.SetActive(false);
            // in the template, "dice" combines a box and a sphere
            var dice = world.root.GetComponentsInChildren<me.tooster.sdf.Operators.SdfCombineController>(true).First(c => c.name == "dice");
            dice.Operation = me.tooster.sdf.Operators.SdfCombineController.CombinationOperation.SIMPLE_UNION;
            world.Regenerate();
            var union = world.Render().Count(c => c.r + c.g + c.b > 0);
            dice.Operation = me.tooster.sdf.Operators.SdfCombineController.CombinationOperation.SUBTRACTION;
            world.Regenerate();
            var subtraction = world.Render().Count(c => c.r + c.g + c.b > 0);

            Assert.That(subtraction, Is.GreaterThan(0), "nothing is left");
            Assert.That(subtraction, Is.LessThan(union), "the subtraction covers as much as the union");
        }

        [Test]
        public void Repeat_ShowsCopiesOfTheObject() {
            world.ground.SetActive(false);
            // in the template, the sphere is intersected with a box, which would cut its copies away
            world.root.GetComponentsInChildren<SdfCombineController>(true).First(c => c.name == "dice").Operation =
                SdfCombineController.CombinationOperation.SIMPLE_UNION;
            world.Regenerate();
            var single = world.Render().Count(c => c.r + c.g + c.b > 0);
            var sphere = world.root.GetComponentInChildren<me.tooster.sdf.Primitives.SdfSphereController>(true);
            var repeat = sphere.gameObject.AddComponent<SdfRepeatController>();
            repeat.Period = new Vector3(1.2f, 10, 10);
            repeat.Count = new Vector3(1, 0, 0);
            world.Regenerate();

            Assert.That(world.Render().Count(c => c.r + c.g + c.b > 0), Is.GreaterThan(single), "the copies don't show");
        }

        [Test]
        public void Dye_PaintsTheFirstChildWithoutChangingItsShape() {
            world.ground.SetActive(false);
            // in the template, "dice" combines a box and a sphere that crosses the box's surface
            var dice = world.root.GetComponentsInChildren<SdfCombineController>(true).First(c => c.name == "dice");
            var children = dice.Children;
            dice.Operation = SdfCombineController.CombinationOperation.DYE;
            // a narrow edge, so that the inside of the dye is fully dyed
            dice.BlendFactor = 0.05f;
            // the dyes are red, the first child keeps the default white
            foreach (var dye in children.Skip(1))
                dye.gameObject.AddComponent<SdfMaterialController>().Albedo = Color.red;
            world.Regenerate();
            var dyed = world.Render();
            var dyedCoverage = dyed.Select(c => c.r + c.g + c.b > 0).ToArray();

            // inactive children are still children of a combination
            foreach (var dye in children.Skip(1))
                Object.DestroyImmediate(dye.gameObject);
            world.Regenerate();
            var firstOnly = world.Render().Select(c => c.r + c.g + c.b > 0).ToArray();

            Assert.That(dyedCoverage.Where((c, i) => c != firstOnly[i]).Count(), Is.LessThan(dyedCoverage.Count(c => c) / 50), "the dyes change the shape");
            Assert.That(dyed.Count(c => c.r > 2 * c.g + 20), Is.GreaterThan(10), "nothing is painted red");
        }

        [Test]
        public void DeferredShading_WritesTheSdfIntoTheGBuffer() {
            world.ground.SetActive(false);
            var sdfMask = world.Render().Select(c => c.r + c.g + c.b > 0).ToArray();
            var sdfPixels = sdfMask.Count(c => c);
            Assert.That(sdfPixels, Is.GreaterThan(100), "the SDF isn't visible");

            // copy the albedo buffer right after the G-buffer pass
            var albedo = new RenderTexture(RenderTestScene.Size, RenderTestScene.Size, 0, RenderTextureFormat.ARGB32);
            var copy = new CommandBuffer { name = "copy albedo" };
            copy.Blit(BuiltinRenderTextureType.GBuffer0, albedo);
            world.camera.renderingPath = RenderingPath.DeferredShading;
            world.camera.AddCommandBuffer(CameraEvent.AfterGBuffer, copy);
            try {
                var shaded = world.Render();
                var readback = new Texture2D(RenderTestScene.Size, RenderTestScene.Size, TextureFormat.RGBA32, false);
                RenderTexture.active = albedo;
                readback.ReadPixels(new Rect(0, 0, RenderTestScene.Size, RenderTestScene.Size), 0, 0);
                RenderTexture.active = null;
                var written = readback.GetPixels32().Where((c, i) => sdfMask[i] && c.r + c.g + c.b > 0).Count();
                Object.DestroyImmediate(readback);

                Assert.That(written, Is.GreaterThan(sdfPixels * 9 / 10), "the deferred pass doesn't write the SDF's albedo");
                Assert.That(shaded.Count(c => c.r + c.g + c.b > 0), Is.GreaterThan(sdfPixels * 9 / 10), "the deferred lights don't show the SDF");
            } finally {
                world.camera.RemoveCommandBuffer(CameraEvent.AfterGBuffer, copy);
                copy.Release();
                Object.DestroyImmediate(albedo);
            }
        }

        [Test]
        public void CurvatureView_ShowsConvexSurfacesInBlue() {
            world.ground.SetActive(false);
            world.scene.rendering.drawMode = DrawMode.Curvature;
            world.scene.UpdateShaderUniforms(forceUpdateAll: true);

            var convex = world.Render().Count(c => c.b > c.r + 40);
            Assert.That(convex, Is.GreaterThan(50), "no convex surface shows in blue");
        }

        [Test]
        public void QuadStepsView_ShowsTheMostStepsOfEachQuad() {
            world.ground.SetActive(false);
            world.scene.rendering.drawMode = DrawMode.Steps;
            world.scene.UpdateShaderUniforms(forceUpdateAll: true);
            var steps = world.Render();
            world.scene.rendering.drawMode = DrawMode.QuadSteps;
            world.scene.UpdateShaderUniforms(forceUpdateAll: true);
            var quadSteps = world.Render();

            // the views go from blue to red as the steps increase
            const int size = RenderTestScene.Size;
            int Red(Color32[] image, int x, int y) => image[x + y * size].r;
            bool Covered(int x, int y) => steps[x + y * size].a > 0;
            int fullQuads = 0, uniformQuads = 0, slowerPixels = 0, pixelsInFullQuads = 0;
            for (var y = 0; y < size; y += 2)
            for (var x = 0; x < size; x += 2) {
                if (!(Covered(x, y) && Covered(x + 1, y) && Covered(x, y + 1) && Covered(x + 1, y + 1)))
                    continue;
                fullQuads++;
                var quad = new[] { (x, y), (x + 1, y), (x, y + 1), (x + 1, y + 1) };
                var mostSteps = quad.Max(p => Red(steps, p.Item1, p.Item2));
                if (quad.All(p => Mathf.Abs(Red(quadSteps, p.Item1, p.Item2) - mostSteps) <= 2))
                    uniformQuads++;
                pixelsInFullQuads += 4;
                slowerPixels += quad.Count(p => Red(quadSteps, p.Item1, p.Item2) > Red(steps, p.Item1, p.Item2) + 2);
            }

            // the template covers about 50 whole quads of the image
            Assert.That(fullQuads, Is.GreaterThan(25), "the SDF isn't visible");
            Assert.That(uniformQuads, Is.GreaterThan(fullQuads * 95 / 100), "the quads don't show the steps of their slowest pixel");
            Assert.That(slowerPixels, Is.GreaterThan(0), "no pixel waits for another pixel of its quad");
        }

        [Test]
        public void AlbedoMap_ColorsTheSurface() {
            world.ground.SetActive(false);
            var map = new Texture2D(1, 1);
            map.SetPixel(0, 0, Color.red);
            map.Apply();
            try {
                // a white material, so only the map makes the surface red
                var material = world.root.GetComponentInChildren<SdfCombineController>(true).gameObject.AddComponent<SdfMaterialController>();
                material.AlbedoMap = map;
                world.Regenerate();

                var covered = world.Render().Where(c => c.r + c.g + c.b > 0).ToList();
                Assert.That(covered.Count, Is.GreaterThan(100), "the SDF isn't visible");
                Assert.That(covered.Average(c => c.r), Is.GreaterThan(2 * covered.Average(c => c.g)), "the map doesn't color the surface");
            } finally {
                Object.DestroyImmediate(map);
            }
        }

        [Test]
        public void OcclusionJitter_ReplacesBandsWithNoise() {
            world.ground.SetActive(false);
            world.scene.rendering.drawMode = DrawMode.Occlusion;
            world.scene.rendering.occlusionSteps = 3;
            int Levels(bool jitter) {
                world.scene.rendering.occlusionJitter = jitter;
                world.scene.UpdateShaderUniforms(forceUpdateAll: true);
                return world.Render().Where(c => c.r + c.g + c.b > 0).Select(c => c.r).Distinct().Count();
            }

            Assert.That(Levels(true), Is.GreaterThan(Levels(false)), "jitter doesn't add gray levels to the occlusion");
        }

        [Test]
        public void MovingTheScene_OnlyChangesItsOwnMatrix() {
            world.ground.SetActive(false);
            var transformController = world.root.GetComponentInChildren<TransformController>(true);
            var spaceTransform = transformController.SpaceTransform;
            float CentroidX(Color32[] image)
                => (float)image.Select((c, i) => (c, x: i % RenderTestScene.Size)).Where(p => p.c.r + p.c.g + p.c.b > 0).Average(p => p.x);
            var before = CentroidX(world.Render());

            world.root.transform.position += world.camera.transform.right * 0.5f;
            // the scene's own update, without updates of its controllers
            world.scene.UpdateShaderUniforms();
            var after = CentroidX(world.Render());

            for (var column = 0; column < 4; column++)
                Assert.That(Vector4.Distance(transformController.SpaceTransform.GetColumn(column), spaceTransform.GetColumn(column)), Is.LessThan(1e-4f),
                    "moving the scene changes the transform of a controller");
            Assert.That(after - before, Is.GreaterThan(3), "the SDF doesn't move with the scene");
        }

        [Test]
        public void Volume_RendersLikeTheShapeItStores() {
            world.ground.SetActive(false);
            var combine = world.root.GetComponentInChildren<SdfCombineController>(true);
            foreach (var child in combine.Children.ToList())
                Object.DestroyImmediate(child.gameObject);
            var texture = SphereVolume(32, 2, 0.6f);
            try {
                var volume = global::me.tooster.sdf.Editor.Inspectors.Primitives.SdfPrimitiveControllerEditor
                    .InstantiatePrimitive<me.tooster.sdf.Primitives.SdfVolumeController>("volume", new UnityEditor.MenuCommand(combine.gameObject))
                    .GetComponent<me.tooster.sdf.Primitives.SdfVolumeController>();
                volume.Volume = texture;
                volume.Size = 2 * Vector3.one;
                volume.DistanceScale = 2;
                world.Regenerate();
                var fromVolume = world.Render().Select(c => c.r + c.g + c.b > 0).ToArray();

                Object.DestroyImmediate(volume.gameObject);
                global::me.tooster.sdf.Editor.Inspectors.Primitives.SdfPrimitiveControllerEditor
                    .InstantiatePrimitive<me.tooster.sdf.Primitives.SdfSphereController>("sphere", new UnityEditor.MenuCommand(combine.gameObject))
                    .GetComponent<me.tooster.sdf.Primitives.SdfSphereController>().Radius = 0.6f;
                world.Regenerate();
                var fromSphere = world.Render().Select(c => c.r + c.g + c.b > 0).ToArray();

                var covered = fromSphere.Count(c => c);
                Assert.That(covered, Is.GreaterThan(100), "the sphere isn't visible");
                Assert.That(fromVolume.Where((c, i) => c != fromSphere[i]).Count(), Is.LessThan(covered / 25), "the volume doesn't look like the sphere");
            } finally {
                Object.DestroyImmediate(texture);
            }
        }

        /// A texture of n^3 texels with the SDF of a sphere in a box of the given size, divided by the size
        static Texture3D SphereVolume(int n, float size, float radius) {
            var distances = new float[n * n * n];
            for (var z = 0; z < n; z++)
            for (var y = 0; y < n; y++)
            for (var x = 0; x < n; x++) {
                var p = ((new Vector3(x, y, z) + 0.5f * Vector3.one) / n - 0.5f * Vector3.one) * size;
                distances[x + n * (y + n * z)] = (p.magnitude - radius) / size;
            }
            var texture = new Texture3D(n, n, n, TextureFormat.RFloat, false);
            texture.SetPixelData(distances, 0);
            texture.Apply(false);
            return texture;
        }

        [Test]
        public void ScaledObject_MarchesLikeTheSameObjectWithoutScale() {
            world.ground.SetActive(false);
            var combine = world.root.GetComponentInChildren<SdfCombineController>(true);
            foreach (var child in combine.Children.ToList())
                Object.DestroyImmediate(child.gameObject);
            var sphere = global::me.tooster.sdf.Editor.Inspectors.Primitives.SdfPrimitiveControllerEditor
                .InstantiatePrimitive<me.tooster.sdf.Primitives.SdfSphereController>("sphere", new UnityEditor.MenuCommand(combine.gameObject))
                .GetComponent<me.tooster.sdf.Primitives.SdfSphereController>();
            world.scene.rendering.drawMode = DrawMode.Steps;
            sphere.Radius = 0.6f;
            world.Regenerate();
            var unscaled = world.Render();

            sphere.Radius = 0.3f;
            sphere.transform.localScale = 2 * Vector3.one;
            world.scene.UpdateShaderUniforms(forceUpdateAll: true);
            var scaled = world.Render();

            // the steps view goes from blue to red as the steps increase
            var covered = unscaled.Count(c => c.a > 0);
            Assert.That(covered, Is.GreaterThan(100), "the sphere isn't visible");
            Assert.That(DifferentPixels(scaled, unscaled), Is.LessThan(covered / 50), "the scaled sphere marches differently");
        }

        [Test]
        public void BakedValues_RenderLikeUniforms() {
            var withUniforms = world.Render();
            world.scene.bakeValues = true;
            world.Regenerate();
            var baked = world.Render();

            var sphere = world.root.GetComponentInChildren<me.tooster.sdf.Primitives.SdfSphereController>(true);
            sphere.Radius += 0.5f;
            world.scene.UpdateShaderUniforms();
            var bakedBeforeRegeneration = world.Render();
            world.Regenerate();
            var bakedAfterRegeneration = world.Render();

            Assert.That(DifferentPixels(baked, withUniforms), Is.LessThan(RenderTestScene.Size), "the baked scene looks different");
            Assert.That(DifferentPixels(bakedBeforeRegeneration, baked), Is.Zero, "the baked shader still reads a uniform");
            Assert.That(DifferentPixels(bakedAfterRegeneration, baked), Is.GreaterThan(100), "the new shader doesn't bake the new value");
        }

        static int DifferentPixels(Color32[] image, Color32[] reference)
            => image.Where((c, i) => Mathf.Abs(c.r - reference[i].r) + Mathf.Abs(c.g - reference[i].g) + Mathf.Abs(c.b - reference[i].b) > 6).Count();

        static int DarkerPixels(Color32[] image, Color32[] reference, bool[] mask = null)
            => image.Where((c, i) => (mask == null || mask[i]) && Luminance(reference[i]) - Luminance(c) > 0.1f).Count();

        static float Luminance(Color32 c) => (0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b) / 255f;
    }
}
