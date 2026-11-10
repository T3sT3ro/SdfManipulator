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
    }
}
