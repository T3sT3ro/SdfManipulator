using System.Collections.Generic;
using System.ComponentModel;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;
using Type = System.Type;

namespace me.tooster.sdf.Editor.Controllers.SDF.Operators {
    /// Round SDF, simply by jumping to another isosurface
    [GeneratePropertyBag]
    public partial class SdfOnionController : Controller, IModifier<ScalarData, ScalarData> {
        public enum OnionVariant {
            [Description("Shell")]  SHELL,
            [Description("Layers")] LAYERS,
        }



        static readonly PropertyPath thicknessProperty = new(nameof(Thickness));
        static readonly PropertyPath layersProperty    = new(nameof(Layers));


        [SerializeField] [DontCreateProperty] OnionVariant variant;
        [SerializeField] [DontCreateProperty] float        thickness;
        [SerializeField] [DontCreateProperty] [Range(0, 32)]
        int layers;

        [CreateProperty] [ShaderStructural]
        public OnionVariant Variant {
            get => variant;
            set => SetField(ref variant, value, false);
        }

        [CreateProperty] [ShaderProperty(Description = "Thickness")]
        public float Thickness {
            get => thickness;
            set => SetField(ref thickness, value, false);
        }

        [CreateProperty] [ShaderProperty(Description = "Layers")]
        [VisibleWhen(nameof(variant), (int)OnionVariant.LAYERS)]
        public int Layers {
            get => layers;
            set => SetField(ref layers, value, false);
        }

        public ScalarData Apply(ScalarData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl"));

            var arguments = new List<AST.Syntax.CommonSyntax.Expression<hlsl>>
            {
                input.evaluationExpression,
                (Identifier)this[thicknessProperty].identifier,
            };

            if (Variant == OnionVariant.LAYERS)
                arguments.Add((Identifier)this[layersProperty].identifier);

            return new ScalarData
            {
                evaluationExpression = FunctionCall(
                    "sdf::operators::onion_skin",
                    arguments
                ),
            };
        }

        public override IData Apply(IData input, Processor processor) => Apply((ScalarData)input, processor);
        public override Type  GetInputType()                          => typeof(ScalarData);
        public override Type  GetOutputType()                         => typeof(ScalarData);
    }
}
