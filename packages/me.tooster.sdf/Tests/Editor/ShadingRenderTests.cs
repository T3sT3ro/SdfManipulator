using System.Linq;
using me.tooster.sdf.Util;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>
    ///     Renders the template scene above a ground plane with a directional light, and compares images with and without
    ///     shadow casters. Needs a graphics device, so it's skipped when Unity runs with <c>-nographics</c>.
    /// </summary>
    public class ShadingRenderTests {
        const int Size = 128;

        GameObject    templateRoot;
        Shader        shader;
        Material      material;
        RenderTexture target;
        Texture2D     readback;
        Camera        camera;
        Light         light;
        GameObject    ground;

        ShadowQuality savedShadows;
        float         savedShadowDistance;
        bool          savedAsyncCompilation;

        [SetUp]
        public void SetUp() {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
                Assert.Ignore("Rendering needs a graphics device.");

            savedShadows = QualitySettings.shadows;
            savedShadowDistance = QualitySettings.shadowDistance;
            savedAsyncCompilation = ShaderUtil.allowAsyncCompilation;
            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.shadowDistance = 40;
            // the first frame would otherwise render with a placeholder shader
            ShaderUtil.allowAsyncCompilation = false;

            templateRoot = PrefabUtility.LoadPrefabContents(SdfPaths.Templates.HelloSdfPrefab);
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            scene.RevalidateScene();
            shader = ShaderUtil.CreateShaderAsset(scene.AssembleShaderSource());
            material = new Material(shader);
            foreach (var renderer in templateRoot.GetComponentsInChildren<Renderer>(true))
                renderer.sharedMaterial = material;
            scene.UpdateShaderUniforms(forceUpdateAll: true);

            ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.transform.position = new Vector3(0, -1.5f, 0);

            light = new GameObject("light").AddComponent<Light>();
            light.type = LightType.Directional;
            light.shadows = LightShadows.Hard;
            light.shadowStrength = 1;
            // from above and behind the scene, so that its shadow falls toward the camera
            light.transform.rotation = Quaternion.Euler(70, 180, 0);

            target = new RenderTexture(Size, Size, 24, RenderTextureFormat.ARGB32);
            readback = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            camera = new GameObject("camera").AddComponent<Camera>();
            camera.transform.position = new Vector3(0, 5, -7);
            camera.transform.LookAt(new Vector3(0, -1, 0));
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.targetTexture = target;

            // the prefab contents live in a preview scene, which only its own cameras and lights render
            var previewScene = templateRoot.scene;
            foreach (var go in new[] { ground, light.gameObject, camera.gameObject })
                SceneManager.MoveGameObjectToScene(go, previewScene);
            camera.scene = previewScene;
        }

        [TearDown]
        public void TearDown() {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
                return;
            QualitySettings.shadows = savedShadows;
            QualitySettings.shadowDistance = savedShadowDistance;
            ShaderUtil.allowAsyncCompilation = savedAsyncCompilation;
            foreach (var o in new Object[] { camera != null ? camera.gameObject : null, light != null ? light.gameObject : null, ground, target, readback, material, shader })
                if (o != null)
                    Object.DestroyImmediate(o);
            if (templateRoot != null)
                PrefabUtility.UnloadPrefabContents(templateRoot);
        }

        [Test]
        public void SdfScene_CastsShadowOnOtherGeometry() {
            var sdfRenderers = templateRoot.GetComponentsInChildren<Renderer>(true);

            foreach (var r in sdfRenderers)
                r.shadowCastingMode = ShadowCastingMode.Off;
            var withoutShadow = Render();
            foreach (var r in sdfRenderers)
                r.shadowCastingMode = ShadowCastingMode.On;
            var withShadow = Render();

            Assert.That(DarkerPixels(withShadow, withoutShadow), Is.GreaterThan(100), "the SDF casts no shadow on the ground");
        }

        [Test]
        public void SdfScene_ReceivesShadowOfOtherGeometry() {
            ground.SetActive(false);
            var sdfMask = Render().Select(c => c.r + c.g + c.b > 0).ToArray();
            Assert.That(sdfMask.Count(covered => covered), Is.GreaterThan(100), "the SDF isn't visible");

            // a slab between the light and the scene
            var occluder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            SceneManager.MoveGameObjectToScene(occluder, templateRoot.scene);
            occluder.transform.position = -light.transform.forward * 3.5f;
            occluder.transform.rotation = light.transform.rotation;
            occluder.transform.localScale = new Vector3(3, 3, 0.2f);
            try {
                var occluderRenderer = occluder.GetComponent<Renderer>();
                occluderRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                var shadowed = Render();
                occluderRenderer.enabled = false;
                var lit = Render();

                // surfaces that face away from the light are dark either way
                var darkerSdfPixels = DarkerPixels(shadowed, lit, sdfMask);
                Assert.That(darkerSdfPixels, Is.GreaterThan(sdfMask.Count(covered => covered) / 4), "the SDF isn't shadowed");
            } finally {
                Object.DestroyImmediate(occluder);
            }
        }

        [Test]
        public void SdfScene_ShadesObjectsWithTheirAlbedo() {
            ground.SetActive(false);
            foreach (var sdf in templateRoot.GetComponentsInChildren<SdfController>(true))
                sdf.Albedo = Color.red;
            templateRoot.GetComponentInChildren<SdfScene>(true).UpdateShaderUniforms(forceUpdateAll: true);

            var covered = Render().Where(c => c.r + c.g + c.b > 0).ToList();
            Assert.That(covered.Count, Is.GreaterThan(100), "the SDF isn't visible");
            // ambient light from the sky adds some green and blue
            Assert.That(covered.Average(c => c.r), Is.GreaterThan(2 * covered.Average(c => c.g)));
            Assert.That(covered.Average(c => c.r), Is.GreaterThan(2 * covered.Average(c => c.b)));
        }

        Color32[] Render() {
            camera.Render();
            var previous = RenderTexture.active;
            RenderTexture.active = target;
            readback.ReadPixels(new Rect(0, 0, Size, Size), 0, 0);
            readback.Apply();
            RenderTexture.active = previous;
            return readback.GetPixels32();
        }

        static int DarkerPixels(Color32[] image, Color32[] reference, bool[] mask = null)
            => image.Where((c, i) => (mask == null || mask[i]) && Luminance(reference[i]) - Luminance(c) > 0.1f).Count();

        static float Luminance(Color32 c) => (0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b) / 255f;
    }
}
