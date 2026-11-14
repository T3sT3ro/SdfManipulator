#nullable enable
using System;
using me.tooster.sdf.Data;
using Unity.Properties;
using UnityEngine;

namespace me.tooster.sdf {
    /// <summary>
    ///     The material of SDF objects, with the properties of Unity's Standard shader. It applies to the SDF object on the
    ///     same game object and to the objects below it in the hierarchy that have no material of their own, so one
    ///     material on the root combination colors the whole scene. Objects without a material in their hierarchy are white.
    ///     The values are uniforms, so changing them doesn't regenerate the shader; adding, removing, enabling or disabling
    ///     the component does.
    /// </summary>
    [AddComponentMenu("SDF/Material")]
    [GeneratePropertyBag]
    public partial class SdfMaterialController : Controller {
        [SerializeField] [DontCreateProperty] Color albedo = Color.white;
        [SerializeField] [DontCreateProperty] [Range(0, 1)] float metallic;
        [SerializeField] [DontCreateProperty] [Range(0, 1)] float smoothness = 0.5f;
        [SerializeField] [DontCreateProperty] [ColorUsage(false, true)] Color emission = Color.black;
        [Tooltip("Multiplies the albedo. Projected onto the surface in the space of this object, from the two axes that face the surface most")]
        [SerializeField] [DontCreateProperty] Texture2D? albedoMap;
        [Tooltip("The size of one repetition of the albedo map, in the units of this object's space")]
        [SerializeField] [DontCreateProperty] [Min(0.0001f)] float mapSize = 1;

        bool disabledBefore;

        /// The base color of the surface, as in the Standard shader
        [CreateProperty] [ShaderProperty(Description = "Albedo")]
        public Color Albedo {
            get => albedo;
            set => SetField(ref albedo, value, false);
        }

        [CreateProperty] [ShaderProperty(Description = "Metallic")]
        public float Metallic {
            get => metallic;
            set => SetField(ref metallic, value, false);
        }

        [CreateProperty] [ShaderProperty(Description = "Smoothness")]
        public float Smoothness {
            get => smoothness;
            set => SetField(ref smoothness, value, false);
        }

        /// Light that the surface emits, in HDR
        [CreateProperty] [ShaderProperty(Description = "Emission")]
        public Color Emission {
            get => emission;
            set => SetField(ref emission, value, false);
        }

        [CreateProperty] [ShaderProperty(Description = "Albedo map")]
        public Texture2D? AlbedoMap {
            get => albedoMap;
            set => SetField(ref albedoMap, value, false);
        }

        [CreateProperty] [ShaderProperty(Description = "Map size")]
        public float MapSize {
            get => mapSize;
            set => SetField(ref mapSize, value, false);
        }

        /// The space in which the albedo map is projected, relative to the scene's root, so the texture moves with this object
        [CreateProperty] [ShaderProperty(Description = "Map space")]
        public Matrix4x4 MapSpace
            => SdfScene != null ? transform.worldToLocalMatrix * SdfScene.transform.localToWorldMatrix : transform.worldToLocalMatrix;

        protected virtual void Update() {
            if (transform.hasChanged)
                OnPropertyChanged(nameof(MapSpace));
        }

        protected virtual void LateUpdate() => transform.hasChanged = false;

        // a disabled material doesn't apply, so toggling the component changes the generated functions; loading doesn't
        protected override void OnEnable() {
            base.OnEnable();
            if (disabledBefore)
                OnStructureChanged();
        }

        void OnDisable() {
            disabledBefore = true;
            OnStructureChanged();
        }

        // a material isn't part of the modifier stack of an SDF; SdfController reads its uniforms
        public override IData Apply(IData input, Processor processor) => input;
        public override Type  GetInputType()                          => typeof(IData);
        public override Type  GetOutputType()                         => typeof(IData);
    }
}
