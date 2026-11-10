using me.tooster.sdf.Primitives;
using me.tooster.sdf.Util;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>Uniforms go to each renderer of a scene through a property block, not into the material.</summary>
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
        public void Update_SetsUniformsOnTheRendererAndLeavesTheMaterial() {
            var sphere = root.GetComponentInChildren<SdfSphereController>(true);
            var shader = ShaderUtil.CreateShaderAsset(scene.AssembleShaderSource());
            var material = new Material(shader);
            try {
                scene.GetComponentInChildren<Renderer>(true).sharedMaterial = material;
                var generatedRadius = sphere.Radius;
                sphere.Radius = generatedRadius + 0.25f;

                scene.UpdateShaderUniforms(forceUpdateAll: true);

                Assert.That(RadiusInPropertyBlock(sphere), Is.EqualTo(sphere.Radius));
                Assert.That(material.GetFloat(RadiusId(sphere)), Is.EqualTo(generatedRadius).Within(1e-6), "the uniform was written into the material");
            } finally {
                Object.DestroyImmediate(material);
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
