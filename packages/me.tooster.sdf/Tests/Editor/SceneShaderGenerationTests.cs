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
