using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.Generators;
using me.tooster.sdf.Operators;
using me.tooster.sdf.Primitives;
using me.tooster.sdf.Util;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TestTools;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>
    ///     Generates the shader for the package's new-scene template and compiles it. Covers preset deserialization, the
    ///     generators, and every include path the generated shader references.
    /// </summary>
    public class SceneShaderGenerationTests {
        GameObject templateRoot;
        Shader     shader;

        [SetUp]
        public void LoadTemplate() => templateRoot = PrefabUtility.LoadPrefabContents(SdfPaths.Templates.HelloSdfPrefab);

        [TearDown]
        public void Unload() {
            if (shader != null)
                Object.DestroyImmediate(shader);
            if (templateRoot != null)
                PrefabUtility.UnloadPrefabContents(templateRoot);
        }

        [Test]
        public void TemplateScene_HasBuiltInShaderPreset() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            Assert.That(scene, Is.Not.Null, "The template has no SdfScene");
            Assert.That(scene.shaderPreset, Is.InstanceOf<BIRPShaderPreset>());
        }

        [Test]
        public void TemplateScene_GeneratesShaderThatCompiles() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            scene.RevalidateScene();
            var source = scene.AssembleShaderSource();

            // library includes declare keywords with #pragma, which Unity only honors through #include_with_pragmas
            StringAssert.Contains($"#include_with_pragmas \"{SdfPaths.ShaderLibrary.Raymarching}\"", source);

            shader = ShaderUtil.CreateShaderAsset(source);
            Assert.That(CompileErrors(shader, source), Is.Empty);
        }

        [Test]
        public void TemplateScene_ShaderDeclaresLibraryKeywords() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            scene.RevalidateScene();
            shader = ShaderUtil.CreateShaderAsset(scene.AssembleShaderSource());

            foreach (var keyword in new[] { "_DRAWMODE_NORMAL", "_SHOW_WORLD_GRID", "_ZWRITE_ON" })
                Assert.That(shader.keywordSpace.FindKeyword(keyword).isValid, Is.True, $"keyword {keyword} isn't declared");
        }

        // commands and keyword drawers read Float properties, and the legacy Int type also reports Float
        [TestCase("_Cull", ShaderPropertyType.Float)]
        [TestCase("_ZTest", ShaderPropertyType.Float)]
        [TestCase("_DrawMode", ShaderPropertyType.Float)]
        [TestCase("_MAX_STEPS", ShaderPropertyType.Int)]
        [TestCase("_AO_STEPS", ShaderPropertyType.Int)]
        public void TemplateScene_RenderingSettingHasType(string property, ShaderPropertyType type) {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            scene.RevalidateScene();
            shader = ShaderUtil.CreateShaderAsset(scene.AssembleShaderSource());

            var index = shader.FindPropertyIndex(property);
            Assert.That(index, Is.Not.EqualTo(-1), $"{property} isn't a material property");
            Assert.That(shader.GetPropertyType(index), Is.EqualTo(type));
        }

        [Test]
        public void TemplateScene_ShaderHasBuiltInPipelinePasses() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            scene.RevalidateScene();
            shader = ShaderUtil.CreateShaderAsset(scene.AssembleShaderSource());

            Assert.That(LightModes(shader), Is.EqualTo(new[] { "ForwardBase", "ForwardAdd", "ShadowCaster" }).IgnoreCase);
        }

        [Test]
        public void TemplateScene_PresetPragmasOnlySelectTheForwardBasePass() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            scene.RevalidateScene();
            var source = scene.AssembleShaderSource();

            Assert.That(Occurrences(source, "#pragma vertex vertexShader"), Is.EqualTo(1));
            Assert.That(Occurrences(source, "#pragma vertex shadowCasterVertex"), Is.EqualTo(1));
            Assert.That(Occurrences(source, "#pragma vertex forwardAddVertex"), Is.EqualTo(1));
            // the scene functions are defined once, in the block that the passes share
            Assert.That(Occurrences(source, "SdfResult sdfScene(float3 p) {"), Is.EqualTo(1));
        }

        // keywords that the light and shadow settings of a scene enable, which the variants without keywords don't cover
        static readonly object[] LightingVariants =
        {
            new object[] { "ForwardBase", new[] { "DIRECTIONAL", "SHADOWS_SCREEN", "LIGHTPROBE_SH", "FOG_EXP2" } },
            new object[] { "ForwardBase", new[] { "DIRECTIONAL", "LIGHTPROBE_SH", "VERTEXLIGHT_ON" } },
            new object[] { "ForwardAdd", new[] { "POINT", "SHADOWS_CUBE", "SHADOWS_SOFT", "FOG_LINEAR" } },
            new object[] { "ForwardAdd", new[] { "SPOT", "SHADOWS_DEPTH" } },
            new object[] { "ForwardAdd", new[] { "POINT_COOKIE" } },
            new object[] { "ForwardAdd", new[] { "DIRECTIONAL_COOKIE", "SHADOWS_SCREEN" } },
            new object[] { "ShadowCaster", new[] { "SHADOWS_DEPTH" } },
            new object[] { "ShadowCaster", new[] { "SHADOWS_CUBE" } },
        };

        [TestCaseSource(nameof(LightingVariants))]
        public void TemplateScene_PassCompilesWithLightingKeywords(string lightMode, string[] keywords) {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            scene.RevalidateScene();
            shader = ShaderUtil.CreateShaderAsset(scene.AssembleShaderSource());

            var passIndex = LightModes(shader).FindIndex(mode => string.Equals(mode, lightMode, System.StringComparison.OrdinalIgnoreCase));
            Assert.That(passIndex, Is.Not.EqualTo(-1), $"no {lightMode} pass");
            var pass = ShaderUtil.GetShaderData(shader).GetSubshader(0).GetPass(passIndex);
            var errors = new List<string>();
            // OpenGL has no reversed depth, so it covers the other near plane value
            foreach (var platform in new[] { ShaderCompilerPlatform.Vulkan, ShaderCompilerPlatform.OpenGLCore })
            foreach (var stage in new[] { ShaderType.Vertex, ShaderType.Fragment })
                errors.AddRange(
                    pass.CompileVariant(stage, keywords, platform, BuildTarget.StandaloneLinux64)
                        .Messages.Where(message => message.severity == ShaderCompilerMessageSeverity.Error)
                        .Select(message => $"{platform} {stage}: {message.file}:{message.line}: {message.message}")
                );
            Assert.That(errors, Is.Empty);
        }

        [Test]
        public void Generation_KeepsElongatedObjectName() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            var combine = templateRoot.GetComponentInChildren<SdfCombineController>(true);

            var elongated = new GameObject("elongated sphere");
            elongated.transform.SetParent(combine.transform, false);
            var sphere = elongated.AddComponent<SdfSphereController>();
            // OnValidate runs on AddComponent, before the target can be assigned
            LogAssert.Expect(LogType.Error, "sdf elongate requires an sdf controller as a target!");
            var elongate = elongated.AddComponent<SdfElongateController>();
            elongate.sdfPrimitive = sphere;
            var sdf = elongated.AddComponent<SdfController>();
            sdf.sdfModifiers = new Controller[] { elongate };

            scene.RevalidateScene();
            var source = scene.AssembleShaderSource();

            StringAssert.Contains("q = abs(", source, "the elongate operator isn't part of the generated shader");
            Assert.That(elongated.name, Is.EqualTo("elongated sphere"));
        }

        [Test]
        public void Generation_EmptyCombineCompiles() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            var combine = templateRoot.GetComponentInChildren<SdfCombineController>(true);

            var empty = new GameObject("empty combine");
            empty.transform.SetParent(combine.transform, false);
            var emptyCombine = empty.AddComponent<SdfCombineController>();

            scene.RevalidateScene();
            var source = scene.AssembleShaderSource();

            StringAssert.Contains("result.distance = _MAX_DISTANCE;", source);
            shader = ShaderUtil.CreateShaderAsset(source);
            Assert.That(CompileErrors(shader, source), Is.Empty);
        }

        [Test]
        public void Generation_DoesNotDependOnUniformValues() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            scene.RevalidateScene();
            var source = scene.AssembleShaderSource();

            templateRoot.GetComponentInChildren<SdfSphereController>(true).Radius += 0.25f;
            templateRoot.GetComponentInChildren<TransformController>(true).transform.position += Vector3.one;
            scene.RevalidateScene();

            Assert.That(scene.AssembleShaderSource(), Is.EqualTo(source), "a value that only sets a uniform changed the shader");
        }

        [Test]
        public void UniformUpdate_WithoutRendererDoesNotThrow() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            foreach (var renderer in templateRoot.GetComponentsInChildren<Renderer>(true))
                Object.DestroyImmediate(renderer);
            scene.RevalidateScene();

            Assert.DoesNotThrow(() => scene.UpdateShaderUniforms(forceUpdateAll: true));
        }

        // Unity reports tag values in upper case
        static List<string> LightModes(Shader shader)
            => Enumerable.Range(0, shader.passCount)
                .Select(pass => shader.FindPassTagValue(pass, new ShaderTagId("LightMode")).name)
                .ToList();

        static int Occurrences(string text, string value) {
            var count = 0;
            for (var index = text.IndexOf(value); index != -1; index = text.IndexOf(value, index + value.Length))
                count++;
            return count;
        }

        static List<string> CompileErrors(Shader shader, string source) {
            var lines = source.Split('\n');
            var errors = ShaderUtil.GetShaderMessages(shader)
                .Where(message => message.severity == ShaderCompilerMessageSeverity.Error)
                .Select(message => $"line {message.line}: {message.message}"
                  + (message.line > 0 && message.line <= lines.Length ? $" | {lines[message.line - 1].Trim()}" : ""))
                .ToList();

            var data = ShaderUtil.GetShaderData(shader);
            for (var s = 0; s < data.SubshaderCount; s++) {
                var subshader = data.GetSubshader(s);
                for (var p = 0; p < subshader.PassCount; p++) {
                    var pass = subshader.GetPass(p);
                    foreach (var stage in new[] { ShaderType.Vertex, ShaderType.Fragment }) {
                        if (!pass.HasShaderStage(stage))
                            continue;
                        var result = pass.CompileVariant(stage, new string[0], ShaderCompilerPlatform.Vulkan, BuildTarget.StandaloneLinux64);
                        errors.AddRange(
                            result.Messages
                                .Where(message => message.severity == ShaderCompilerMessageSeverity.Error)
                                .Select(message => $"pass {p} {stage}: {message.file}:{message.line}: {message.message}")
                        );
                    }
                }
            }
            return errors;
        }
    }
}
