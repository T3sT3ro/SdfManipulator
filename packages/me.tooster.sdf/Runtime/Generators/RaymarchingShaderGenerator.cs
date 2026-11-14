using System;
using System.Linq;
using me.tooster.sdf.Util;
using UnityEngine.Scripting.APIUpdating;
namespace me.tooster.sdf.Generators {
    /// <summary>
    ///     An implementation of a shader preset for the built-in generator
    /// </summary>
    [Serializable]
    [MovedFrom(true, sourceNamespace: "me.tooster.sdf.Editor.Controllers.Generators", sourceAssembly: "me.tooster.sdf.Editor.Controllers")]
    public class BIRPShaderPreset : ShaderPreset {
        /// <summary>
        ///     Pragmas of the forward base pass, which select its vertex and fragment shaders. The other passes have
        ///     their own entry points.
        /// </summary>
        public string[] pragmas =
        {
            "vertex vertexShader",
            "fragment fragmentShader",
        };
        public string[] defines = { };
        [UnityEngine.Tooltip("Leave the debug views out of the generated shader, e.g. for a release. The draw mode and the "
                           + "world grid of the scene's rendering settings then have no effect.")]
        public bool stripDebugViews;
        /// <summary>
        ///     Files that every pass includes. The generator also includes <see cref="SdfPaths.ShaderLibrary.BuiltInShading" />,
        ///     which defines the passes.
        /// </summary>
        public string[] includes =
        {
            SdfPaths.ShaderLibrary.Raymarching,
            SdfPaths.ShaderLibrary.Util,
            SdfPaths.ShaderLibrary.Operators,
            SdfPaths.ShaderLibrary.Matrix,
            SdfPaths.ShaderLibrary.Primitives,
            SdfPaths.ShaderLibrary.BuiltInShading,
        };

        protected override RaymarchingShaderGenerator CreateProcessorForScene(SdfScene scene)
            => new BuiltInGenerator(scene, pragmas, stripDebugViews ? defines.Append("SDF_NO_DEBUG_VIEWS").ToArray() : defines, includes);
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
