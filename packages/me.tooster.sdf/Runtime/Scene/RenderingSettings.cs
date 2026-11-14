using System;
using UnityEngine;

namespace me.tooster.sdf {
    /// <summary>
    ///     How an SDF scene is raymarched and shown. <see cref="SdfScene" /> sets these values on its renderers with its
    ///     property block, so they're saved with the scene, prefab instances can override them, and regenerating the
    ///     shader doesn't reset them. The material only holds the render state: culling, the depth test and depth writes.
    /// </summary>
    [Serializable]
    public class RenderingSettings {
        static readonly int drawModeId          = Shader.PropertyToID("_DrawMode");
        static readonly int showWorldGridId     = Shader.PropertyToID("_SHOW_WORLD_GRID");
        static readonly int maxStepsId          = Shader.PropertyToID("_MAX_STEPS");
        static readonly int maxDistanceId       = Shader.PropertyToID("_MAX_DISTANCE");
        static readonly int hitDistanceId       = Shader.PropertyToID("_EPSILON_RAY");
        static readonly int normalEpsilonId     = Shader.PropertyToID("_EPSILON_NORMAL");
        static readonly int rayOriginBiasId     = Shader.PropertyToID("_RAY_ORIGIN_BIAS");
        static readonly int occlusionStepsId    = Shader.PropertyToID("_AO_STEPS");
        static readonly int occlusionDistanceId = Shader.PropertyToID("_AO_MAX_DISTANCE");
        static readonly int occlusionFalloffId  = Shader.PropertyToID("_AO_FALLOFF");

        [Tooltip("What the scene shows: the shaded materials or a debug view")]
        public DrawMode drawMode = DrawMode.Material;
        [Tooltip("Overlay a grid in world space")]
        public bool showWorldGrid;

        [Header("Raymarching")]
        [Tooltip("The most steps that a ray takes before it stops")]
        [Min(1)] public int maxSteps = 200;
        [Tooltip("The longest distance that a ray marches")]
        [Min(0)] public float maxDistance = 200;
        [Tooltip("A ray hits a surface when it gets closer to it than this distance")]
        [Min(0)] public float hitDistance = 0.001f;
        [Tooltip("The offset at which the SDF is sampled to compute normals")]
        [Min(0)] public float normalEpsilon = 0.001f;
        [Tooltip("Moves the start of each ray forward by this distance")]
        public float rayOriginBias;

        [Header("Ambient occlusion")]
        [Min(0)] public int occlusionSteps = 6;
        [Min(0)] public float occlusionDistance = 1;
        [Min(0)] public float occlusionFalloff = 2.5f;

        public void ApplyTo(MaterialPropertyBlock block) {
            block.SetFloat(drawModeId, (int)drawMode);
            block.SetFloat(showWorldGridId, showWorldGrid ? 1 : 0);
            block.SetInteger(maxStepsId, maxSteps);
            block.SetFloat(maxDistanceId, maxDistance);
            block.SetFloat(hitDistanceId, hitDistance);
            block.SetFloat(normalEpsilonId, normalEpsilon);
            block.SetFloat(rayOriginBiasId, rayOriginBias);
            block.SetInteger(occlusionStepsId, occlusionSteps);
            block.SetFloat(occlusionDistanceId, occlusionDistance);
            block.SetFloat(occlusionFalloffId, occlusionFalloff);
        }

        /// <summary>Sets the values on a material, for drawing without a property block.</summary>
        public void ApplyTo(Material material) {
            material.SetFloat(drawModeId, (int)drawMode);
            material.SetFloat(showWorldGridId, showWorldGrid ? 1 : 0);
            material.SetInteger(maxStepsId, maxSteps);
            material.SetFloat(maxDistanceId, maxDistance);
            material.SetFloat(hitDistanceId, hitDistance);
            material.SetFloat(normalEpsilonId, normalEpsilon);
            material.SetFloat(rayOriginBiasId, rayOriginBias);
            material.SetInteger(occlusionStepsId, occlusionSteps);
            material.SetFloat(occlusionDistanceId, occlusionDistance);
            material.SetFloat(occlusionFalloffId, occlusionFalloff);
        }
    }
}
