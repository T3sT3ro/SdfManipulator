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

        // switching a debug view or depth writes doesn't compile another variant
        [Test]
        public void TemplateScene_DebugViewsAreNotKeywords() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            scene.RevalidateScene();
            shader = ShaderUtil.CreateShaderAsset(scene.AssembleShaderSource());

            foreach (var keyword in new[] { "_DRAWMODE_NORMAL", "_SHOW_WORLD_GRID", "_ZWRITE_ON" })
                Assert.That(shader.keywordSpace.FindKeyword(keyword).isValid, Is.False, $"{keyword} is a keyword");
        }

        // the other settings are uniforms that SdfScene sets, so regenerating the shader doesn't reset them
        [Test]
        public void TemplateScene_MaterialHoldsOnlyTheRenderState() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            scene.RevalidateScene();
            shader = ShaderUtil.CreateShaderAsset(scene.AssembleShaderSource());

            var properties = Enumerable.Range(0, shader.GetPropertyCount()).Select(shader.GetPropertyName);
            Assert.That(properties, Is.EquivalentTo(new[] { "_Cull", "_ZWrite", "_ZTest" }));
            foreach (var property in new[] { "_Cull", "_ZWrite", "_ZTest" })
                Assert.That(shader.GetPropertyType(shader.FindPropertyIndex(property)), Is.EqualTo(ShaderPropertyType.Float));
        }

        [Test]
        public void TemplateScene_ShaderHasBuiltInPipelinePasses() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            scene.RevalidateScene();
            shader = ShaderUtil.CreateShaderAsset(scene.AssembleShaderSource());

            Assert.That(LightModes(shader), Is.EqualTo(new[] { "ForwardBase", "ForwardAdd", "ShadowCaster", "SceneSelectionPass", "Picking", "SdfObjectPicking", "SdfFocus" }).IgnoreCase);
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
        public void Generation_ObjectsWithoutAMaterialUseTheDefault() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            scene.RevalidateScene();
            var source = scene.AssembleShaderSource();

            StringAssert.Contains("result.material = defaultMaterial();", source);
            StringAssert.DoesNotContain("standardMaterial(SDF_", source);
        }

        [Test]
        public void Generation_ObjectsUseTheMaterialOnThemOrTheirNearestParent() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            var rootMaterial = scene.GetComponentInChildren<SdfCombineController>(true).gameObject.AddComponent<SdfMaterialController>();
            var sphereMaterial = templateRoot.GetComponentInChildren<SdfSphereController>(true).gameObject.AddComponent<SdfMaterialController>();
            scene.RevalidateScene();
            var source = scene.AssembleShaderSource();

            string AlbedoOf(SdfMaterialController material)
                => scene.sceneData.controllers[material].properties[new Unity.Properties.PropertyPath(nameof(SdfMaterialController.Albedo))].identifier;
            var objects = templateRoot.GetComponentsInChildren<SdfController>(true).Count(c => c is not SdfCombineController);
            Assert.That(Occurrences(source, $"standardMaterial({AlbedoOf(sphereMaterial)},"), Is.EqualTo(1), "the sphere doesn't use its own material");
            Assert.That(Occurrences(source, $"standardMaterial({AlbedoOf(rootMaterial)},"), Is.EqualTo(objects - 1), "the other objects don't use the root's material");
        }

        [Test]
        public void Generation_DisabledMaterialDoesNotApply() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            var material = scene.GetComponentInChildren<SdfCombineController>(true).gameObject.AddComponent<SdfMaterialController>();
            material.enabled = false;
            scene.RevalidateScene();

            StringAssert.DoesNotContain("standardMaterial(SDF_", scene.AssembleShaderSource());
        }

        // the forward passes evaluate the scene once more at the hit to get the material, so marching only needs distances
        [Test]
        public void ForwardPass_MarchingLoopReadsNoMaterial() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            scene.GetComponentInChildren<SdfCombineController>(true).gameObject.AddComponent<SdfMaterialController>();
            templateRoot.GetComponentInChildren<SdfSphereController>(true).gameObject.AddComponent<SdfMaterialController>();
            scene.RevalidateScene();
            shader = ShaderUtil.CreateShaderAsset(scene.AssembleShaderSource());

            // Metal is the platform whose compiled shaders are source code; the others are SPIR-V or DXBC
            var forwardBase = LightModes(shader).FindIndex(mode => mode == "FORWARDBASE");
            var compiled = ShaderUtil.GetShaderData(shader).GetSubshader(0).GetPass(forwardBase)
                .CompileVariant(ShaderType.Fragment, new[] { "DIRECTIONAL" }, ShaderCompilerPlatform.Metal, BuildTarget.iOS);
            if (!compiled.Success || compiled.ShaderData == null || compiled.ShaderData.Length == 0)
                Assert.Ignore("The Metal shader compiler isn't available.");
            var code = System.Text.Encoding.UTF8.GetString(compiled.ShaderData);

            var start = code.IndexOf("while(true){", System.StringComparison.Ordinal);
            Assert.That(start, Is.Not.EqualTo(-1), "the compiled shader has no loop");
            var end = start + "while(true){".Length;
            for (var depth = 1; depth > 0 && end < code.Length; end++)
                depth += code[end] == '{' ? 1 : code[end] == '}' ? -1 : 0;
            var loop = code.Substring(start, end - start);
            Assert.That(loop, Does.Contain("_EPSILON_RAY"), "the first loop isn't the marching loop");
            foreach (var material in new[] { "_Albedo", "_Metallic", "_Smoothness", "_Emission" })
                Assert.That(loop, Does.Not.Contain(material), $"marching reads {material}");
        }

        [Test]
        public void Generation_EvaluatesEachObjectOnItsOwnById() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            scene.RevalidateScene();
            var source = scene.AssembleShaderSource();

            StringAssert.Contains("SdfResult sdfObject(float3 p, int id) {", source);
            foreach (var sdf in templateRoot.GetComponentsInChildren<SdfController>(true)) {
                var data = scene.sceneData.controllers[sdf];
                StringAssert.Contains($"case {data.numericId}:", source, $"{sdf.name} has no case");
                StringAssert.Contains($"return {data.identifier}(p);", source);
            }
        }

        [TestCase(SdfCombineController.CombinationOperation.SUBTRACTION)]
        [TestCase(SdfCombineController.CombinationOperation.SMOOTH_INTERSECTION)]
        [TestCase(SdfCombineController.CombinationOperation.SMOOTH_SUBTRACTION)]
        [TestCase(SdfCombineController.CombinationOperation.DYE)]
        public void Generation_CombinationOperationCompiles(SdfCombineController.CombinationOperation operation) {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            scene.GetComponentInChildren<SdfCombineController>(true).Operation = operation;
            scene.RevalidateScene();
            var source = scene.AssembleShaderSource();

            shader = ShaderUtil.CreateShaderAsset(source);
            Assert.That(CompileErrors(shader, source), Is.Empty);
        }

        [Test]
        public void Repeat_JoinsTheStackAfterTheTransformAndCompiles() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            var sphere = templateRoot.GetComponentInChildren<SdfSphereController>(true);
            var repeat = sphere.gameObject.AddComponent<SdfRepeatController>();
            scene.RevalidateScene();
            var source = scene.AssembleShaderSource();

            var stack = sphere.GetComponent<SdfController>().sdfModifiers;
            Assert.That(stack, Is.EqualTo(new Controller[] { sphere.GetComponent<TransformController>(), repeat, sphere }));
            StringAssert.Contains("sdf::operators::repeatLimited(", source);
            shader = ShaderUtil.CreateShaderAsset(source);
            Assert.That(CompileErrors(shader, source), Is.Empty);
        }

        [Test]
        public void MirrorAndTwist_JoinTheStackAndCompile() {
            var scene = templateRoot.GetComponentInChildren<SdfScene>(true);
            var sphere = templateRoot.GetComponentInChildren<SdfSphereController>(true);
            var mirror = sphere.gameObject.AddComponent<SdfMirrorController>();
            var twist = sphere.gameObject.AddComponent<SdfTwistController>();
            scene.RevalidateScene();
            var source = scene.AssembleShaderSource();

            Assert.That(sphere.GetComponent<SdfController>().sdfModifiers, Has.Member(mirror).And.Member(twist));
            StringAssert.Contains("sdf::operators::mirror(", source);
            StringAssert.Contains("sdf::operators::twist(", source);
            shader = ShaderUtil.CreateShaderAsset(source);
            Assert.That(CompileErrors(shader, source), Is.Empty);
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
