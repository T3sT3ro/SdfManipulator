using me.tooster.sdf.Primitives;
using me.tooster.sdf.Util;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>Uniforms go to each renderer of a scene through a property block. The material has no properties for them.</summary>
    public class UniformTests {
        GameObject root;
        SdfScene   scene;

        [SetUp]
        public void LoadTemplate() {
            root = PrefabUtility.LoadPrefabContents(SdfPaths.Templates.HelloSdfPrefab);
            scene = root.GetComponentInChildren<SdfScene>(true);
            scene.RevalidateScene();
        }

        [TearDown]
        public void Unload() => PrefabUtility.UnloadPrefabContents(root);

        int RadiusId(SdfSphereController sphere)
            => scene.sceneData.controllers[sphere].properties[new Unity.Properties.PropertyPath(nameof(SdfSphereController.Radius))].shaderId;

        float RadiusInPropertyBlock(SdfSphereController sphere) {
            var block = new MaterialPropertyBlock();
            scene.GetComponentInChildren<Renderer>(true).GetPropertyBlock(block);
            return block.GetFloat(RadiusId(sphere));
        }

        [Test]
        public void ForcedUpdate_SetsUniformsOnTheRenderer() {
            var sphere = root.GetComponentInChildren<SdfSphereController>(true);
            sphere.Radius += 0.25f;

            scene.UpdateShaderUniforms(forceUpdateAll: true);

            Assert.That(RadiusInPropertyBlock(sphere), Is.EqualTo(sphere.Radius));
        }

        [Test]
        public void GeneratedShader_HasNoMaterialPropertiesForControllers() {
            var shader = ShaderUtil.CreateShaderAsset(scene.AssembleShaderSource());
            try {
                foreach (var property in scene.sceneData.Properties)
                    Assert.That(shader.FindPropertyIndex(property.identifier), Is.EqualTo(-1), $"{property.identifier} is a material property");
            } finally {
                Object.DestroyImmediate(shader);
            }
        }

        [Test]
        public void ChangedProperty_UpdatesItsUniform() {
            var sphere = root.GetComponentInChildren<SdfSphereController>(true);
            scene.UpdateShaderUniforms(forceUpdateAll: true);

            sphere.Radius += 0.25f;
            scene.UpdateShaderUniforms();

            Assert.That(RadiusInPropertyBlock(sphere), Is.EqualTo(sphere.Radius));
        }

        [Test]
        public void RenderingSettings_SetUniformsOnTheRenderer() {
            scene.rendering.drawMode = DrawMode.Normal;
            scene.rendering.maxSteps = 77;

            scene.UpdateShaderUniforms(forceUpdateAll: true);

            var block = new MaterialPropertyBlock();
            scene.GetComponentInChildren<Renderer>(true).GetPropertyBlock(block);
            Assert.That(block.GetFloat("_DrawMode"), Is.EqualTo((float)DrawMode.Normal));
            Assert.That(block.GetInteger("_MAX_STEPS"), Is.EqualTo(77));
        }

        [Test]
        public void MaterialProperties_SetUniformsOnTheRenderer() {
            var sdf = root.GetComponentInChildren<SdfSphereController>(true).GetComponent<SdfController>();
            sdf.Albedo = Color.red;
            sdf.Smoothness = 0.25f;

            scene.UpdateShaderUniforms(forceUpdateAll: true);

            var block = new MaterialPropertyBlock();
            scene.GetComponentInChildren<Renderer>(true).GetPropertyBlock(block);
            var properties = scene.sceneData.controllers[sdf].properties;
            Assert.That(block.GetVector(properties[new Unity.Properties.PropertyPath(nameof(SdfController.Albedo))].shaderId), Is.EqualTo((Vector4)Color.red));
            Assert.That(block.GetFloat(properties[new Unity.Properties.PropertyPath(nameof(SdfController.Smoothness))].shaderId), Is.EqualTo(0.25f));
        }
    }
}
