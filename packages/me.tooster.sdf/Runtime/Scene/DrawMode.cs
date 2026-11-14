using UnityEngine;

namespace me.tooster.sdf {
    /// <summary>
    ///     What an SDF scene shows: the shaded materials, or one of the debug views. The values match the
    ///     <c>SDF_DRAWMODE_</c> constants in <c>raymarching.hlsl</c>.
    /// </summary>
    public enum DrawMode {
        Material  = 0,
        Albedo    = 1,
        [InspectorName("Object ID")] Id = 2,
        Skybox    = 3,
        Normal    = 4,
        [InspectorName("Marching steps")] Steps = 5,
        Depth     = 6,
        Occlusion = 7,
        /// red where the surface is concave, blue where it's convex
        Curvature = 8,
        /// the most steps that a pixel in the same 2x2 quad took: the GPU runs the pixels of a quad together, so each
        /// waits for the slowest
        [InspectorName("Marching steps per quad")] QuadSteps = 9,
    }
}
