using System;
using me.tooster.sdf.Util;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>
    ///     The template scene with its generated shader, above a ground plane, with a directional light and a camera that
    ///     renders into a texture. Rendering needs a graphics device, so creating it ignores the test when Unity runs
    ///     with <c>-nographics</c>.
    /// </summary>
    sealed class RenderTestScene : IDisposable {
        public const int Size = 128;

        public readonly GameObject root;
        public readonly SdfScene   scene;
        public readonly Shader     shader;
        public readonly Material   material;
        public readonly GameObject ground;
        public readonly Light      light;
        public readonly Camera     camera;

        readonly RenderTexture target;
        // holds raw values: an sRGB target would convert picking indices, which are encoded as colors
        readonly RenderTexture linearTarget;
        readonly Texture2D     readback;
        readonly ShadowQuality savedShadows;
        readonly float         savedShadowDistance;
        readonly bool          savedAsyncCompilation;

        public RenderTestScene() {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
                Assert.Ignore("Rendering needs a graphics device.");

            savedShadows = QualitySettings.shadows;
            savedShadowDistance = QualitySettings.shadowDistance;
            savedAsyncCompilation = ShaderUtil.allowAsyncCompilation;
            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.shadowDistance = 40;
            // the first frame would otherwise render with a placeholder shader
            ShaderUtil.allowAsyncCompilation = false;

            root = PrefabUtility.LoadPrefabContents(SdfPaths.Templates.HelloSdfPrefab);
            scene = root.GetComponentInChildren<SdfScene>(true);
            scene.RevalidateScene();
            shader = ShaderUtil.CreateShaderAsset(scene.AssembleShaderSource());
            material = new Material(shader);
            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
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
            linearTarget = new RenderTexture(Size, Size, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            readback = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            camera = new GameObject("camera").AddComponent<Camera>();
            camera.transform.position = new Vector3(0, 5, -7);
            camera.transform.LookAt(new Vector3(0, -1, 0));
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.clear;
            camera.targetTexture = target;

            // the prefab contents live in a preview scene, which only its own cameras and lights render
            foreach (var go in new[] { ground, light.gameObject, camera.gameObject })
                Add(go);
            camera.scene = root.scene;
        }

        /// <summary>Moves <paramref name="go" /> into the preview scene of the template.</summary>
        public void Add(GameObject go) => SceneManager.MoveGameObjectToScene(go, root.scene);

        /// <summary>Renders the camera's view.</summary>
        public Color32[] Render() {
            camera.Render();
            return ReadTarget(target);
        }

        /// <summary>
        ///     Draws the scene's renderers from the camera's view with the pass <paramref name="passName" /> of their
        ///     material and their property block, onto a cleared target.
        /// </summary>
        public Color32[] RenderPass(string passName) {
            var commands = new CommandBuffer { name = passName };
            commands.SetRenderTarget(target);
            commands.ClearRenderTarget(true, true, Color.clear);
            commands.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
            foreach (var renderer in root.GetComponentsInChildren<MeshRenderer>(true)) {
                var block = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(block);
                var pass = renderer.sharedMaterial.FindPass(passName);
                Assert.That(pass, Is.Not.EqualTo(-1), $"the material has no {passName} pass");
                commands.DrawMesh(renderer.GetComponent<MeshFilter>().sharedMesh, renderer.localToWorldMatrix, renderer.sharedMaterial, 0, pass, block);
            }
            Graphics.ExecuteCommandBuffer(commands);
            commands.Release();
            return ReadTarget(target);
        }

        /// <summary>
        ///     Runs <paramref name="draw" /> with the target active and cleared and the camera's view and projection loaded,
        ///     the way the scene view prepares immediate drawing while it picks objects.
        /// </summary>
        public Color32[] RenderImmediate(Action draw) {
            var previous = RenderTexture.active;
            RenderTexture.active = linearTarget;
            GL.Clear(true, true, Color.clear);
            GL.PushMatrix();
            GL.LoadProjectionMatrix(camera.projectionMatrix);
            GL.modelview = camera.worldToCameraMatrix;
            try {
                draw();
            } finally {
                GL.PopMatrix();
                RenderTexture.active = previous;
            }
            return ReadTarget(linearTarget);
        }

        Color32[] ReadTarget(RenderTexture source) {
            var previous = RenderTexture.active;
            RenderTexture.active = source;
            readback.ReadPixels(new Rect(0, 0, Size, Size), 0, 0);
            readback.Apply();
            RenderTexture.active = previous;
            return readback.GetPixels32();
        }

        public void Dispose() {
            QualitySettings.shadows = savedShadows;
            QualitySettings.shadowDistance = savedShadowDistance;
            ShaderUtil.allowAsyncCompilation = savedAsyncCompilation;
            foreach (var o in new Object[] { camera.gameObject, light.gameObject, ground, target, linearTarget, readback, material, shader })
                Object.DestroyImmediate(o);
            PrefabUtility.UnloadPrefabContents(root);
        }
    }
}
