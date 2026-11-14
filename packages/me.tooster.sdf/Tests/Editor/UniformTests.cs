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

        // the distance scale is the inverse of the largest singular value of the linear part
        static readonly object[] stretchedMatrices =
        {
            new object[] { Matrix4x4.identity, 1f },
            new object[] { Matrix4x4.TRS(new Vector3(1, 2, 3), Quaternion.Euler(30, 40, 50), 2 * Vector3.one), 0.5f },
            new object[] { Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(10, 70, 20), new Vector3(1, 2, 4)), 0.25f },
            // a shear [[1, 1], [0, 1]] stretches by the golden ratio
            new object[] { new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(1, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 1)), 0.618034f },
        };

        [TestCaseSource(nameof(stretchedMatrices))]
        public void DistanceScale_IsTheInverseOfTheLargestStretch(Matrix4x4 matrix, float expected)
            => Assert.That(TransformController.DistanceScaleOf(matrix), Is.EqualTo(expected).Within(1e-4f));

        [Test]
        public void ScaledObject_ScalesItsDistancesBack() {
            var sphere = root.GetComponentInChildren<SdfSphereController>(true);
            var transformController = sphere.GetComponent<TransformController>();
            var distanceScale = transformController.DistanceScale;

            sphere.transform.localScale *= 2;

            Assert.That(transformController.DistanceScale, Is.EqualTo(2 * distanceScale).Within(1e-4f));
            var identifier = scene.sceneData.controllers[transformController].properties[new Unity.Properties.PropertyPath(nameof(TransformController.DistanceScale))].identifier;
            StringAssert.Contains($"* {identifier}", scene.AssembleShaderSource());
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
            var material = root.GetComponentInChildren<SdfSphereController>(true).gameObject.AddComponent<SdfMaterialController>();
            material.Albedo = Color.red;
            material.Smoothness = 0.25f;
            scene.RevalidateScene();

            scene.UpdateShaderUniforms(forceUpdateAll: true);

            var block = new MaterialPropertyBlock();
            scene.GetComponentInChildren<Renderer>(true).GetPropertyBlock(block);
            var properties = scene.sceneData.controllers[material].properties;
            Assert.That(block.GetVector(properties[new Unity.Properties.PropertyPath(nameof(SdfMaterialController.Albedo))].shaderId), Is.EqualTo((Vector4)Color.red));
            Assert.That(block.GetFloat(properties[new Unity.Properties.PropertyPath(nameof(SdfMaterialController.Smoothness))].shaderId), Is.EqualTo(0.25f));
        }
    }
}
