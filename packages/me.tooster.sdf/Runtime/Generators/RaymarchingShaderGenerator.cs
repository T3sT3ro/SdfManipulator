using System;
using me.tooster.sdf.Editor.Controllers.SDF;
using me.tooster.sdf.Util;
using UnityEngine.Scripting.APIUpdating;
namespace me.tooster.sdf.Editor.Controllers.Generators {
    /// <summary>
    ///     An implementation of a shader preset for the built-in generator
    /// </summary>
    [Serializable]
    [MovedFrom(true, sourceAssembly: "me.tooster.sdf.Editor.Controllers")]
    public class BIRPShaderPreset : ShaderPreset {
        public string[] pragmas =
        {
            "vertex vertexShader",
            "fragment fragmentShader",
        };
        public string[] defines = { };
        public string[] includes =
        {
            SdfPaths.ShaderLibrary.Raymarching,
            SdfPaths.ShaderLibrary.Util,
            SdfPaths.ShaderLibrary.Operators,
            SdfPaths.ShaderLibrary.Matrix,
            SdfPaths.ShaderLibrary.Primitives,
            // SdfPaths.ShaderLibrary.BuiltInForwardBase,
            SdfPaths.ShaderLibrary.DebugBaseShading,
        };

        // SdfPaths.ShaderLibrary.BuiltInForwardBase,

        protected override RaymarchingShaderGenerator CreateProcessorForScene(SdfScene scene)
            => new BuiltInGenerator(scene, pragmas, defines, includes);
    }



    /// <summary>
    ///     A processor for generating raymarching shaders
    /// </summary>
    public abstract class RaymarchingShaderGenerator : Processor {
        protected readonly SdfScene scene;
        protected RaymarchingShaderGenerator(SdfScene scene) => this.scene = scene;

        public abstract string MainShader();
    }
}
