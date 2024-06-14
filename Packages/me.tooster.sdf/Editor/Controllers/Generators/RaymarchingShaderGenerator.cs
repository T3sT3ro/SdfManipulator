using System;
using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
namespace me.tooster.sdf.Editor.Controllers.Generators {
    /// <summary>
    /// An implementation of a shader preset for the built-in generator
    /// </summary>
    [Serializable]
    public class BIRPShaderPreset : ShaderPreset {
        public string[]        customPragmas;
        public string[]        customDefines;
        public ShaderInclude[] customIncludes;

        protected override RaymarchingShaderGenerator CreateProcessorForScene(SdfScene scene) => new BuiltInGenerator(scene);
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
