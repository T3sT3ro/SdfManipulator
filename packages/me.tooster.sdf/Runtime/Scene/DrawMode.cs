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
    }
}
