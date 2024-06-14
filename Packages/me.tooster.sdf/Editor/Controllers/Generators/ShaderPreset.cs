using System;
using System.Linq;
using me.tooster.sdf.Editor.Controllers.SDF;
namespace me.tooster.sdf.Editor.Controllers.Generators {
    /// <summary>
    /// A raymarching shader factory is serialized as a field in the SdfScene. It can persist data and influence the shape of the generated shader
    /// </summary>
    [Serializable]
    public abstract class ShaderPreset {
        /// <summary>Instantiates a new stateful processor for parsing scene data and building the shader</summary>
        /// <param name="scene">scene for the processor to operate on</param>
        /// <returns>a processor for the given scene</returns>
        protected abstract RaymarchingShaderGenerator CreateProcessorForScene(SdfScene scene);

        /// <summary>Generates a source code of the SDF shader for the given scene</summary>
        /// <param name="scene">Scene to generate shader for</param>
        /// <returns>generated main shader text for the given SDF scene</returns>
        public string MainShaderForScene(SdfScene scene) => CreateProcessorForScene(scene).MainShader();

        /// <summary>
        /// Returns detected (on domain reload) concrete classes with parameterless constructor inheriting after ShadePreset 
        /// </summary>
        public static Type[] DetectedShaderPresets { get; } = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(domainAssembly => domainAssembly.GetTypes())
            .Where(
                type => type.IsSubclassOf(typeof(ShaderPreset))
                 && !type.IsAbstract
                 && type.GetConstructor(new Type[] { }) != null
            ).ToArray();

        public static ShaderPreset InstantiatePreset(Type type) => (ShaderPreset)Activator.CreateInstance(type);
    }
}
