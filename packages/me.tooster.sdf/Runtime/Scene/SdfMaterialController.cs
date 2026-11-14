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
