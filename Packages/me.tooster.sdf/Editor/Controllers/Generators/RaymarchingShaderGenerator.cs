using System;
using me.tooster.sdf.Editor.Controllers.SDF;
namespace me.tooster.sdf.Editor.Controllers.Generators {
    /// <summary>
    /// An implementation of a shader preset for the built-in generator
    /// </summary>
    [Serializable]
    public class BIRPShaderPreset : ShaderPreset {
        public string[] pragmas =
        {
            "vertex vertexShader",
            "fragment fragmentShader",
        };
        public string[] defines = { };
        public string[] includes =
        {
            "Packages/me.tooster.sdf/Editor/Resources/Includes/raymarching.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/util.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/matrix.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl",
            // "Packages/me.tooster.sdf/Editor/Resources/Includes/builtInForwardBase.hlsl",
            "Packages/me.tooster.sdf/Editor/Resources/Includes/debugBaseShading.hlsl",
        };

        // "Packages/me.tooster.sdf/Editor/Resources/Includes/builtInForwardBase.hlsl",

        protected override RaymarchingShaderGenerator CreateProcessorForScene(SdfScene scene)
            => new BuiltInGenerator(scene, pragmas, defines, includes);
    }



    /// <summary>
    /// A processor for generating raymarching shaders
    /// </summary>
    public abstract class RaymarchingShaderGenerator : Processor {
        protected readonly SdfScene scene;
        protected RaymarchingShaderGenerator(SdfScene scene) => this.scene = scene;

        public abstract string MainShader();
    }
}
