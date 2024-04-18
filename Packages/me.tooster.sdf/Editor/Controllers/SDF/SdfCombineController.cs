using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.Editor.Controllers.SDF.SdfCombineController.CombinationOperation;


namespace me.tooster.sdf.Editor.Controllers.SDF {
    public partial class SdfCombineController : SdfController {
        public enum CombinationOperation {
            [InspectorName("Simple union")] SIMPLE_UNION = 0,
            [InspectorName("Intersection")] INTERSECTION = 1,
            [InspectorName("Smooth union")] SMOOTH_UNION = 2,
        }



        static readonly PropertyPath blendFactorPropertyPath = new(nameof(BlendFactor));


        [SerializeField] [DontCreateProperty] CombinationOperation operation = SIMPLE_UNION;

        [SerializeField] [DontCreateProperty] float blendFactor = 1;

        [CreateProperty] [ShaderStructural]
        public CombinationOperation Operation {
            get => operation;
            set => SetField(ref operation, value, true);
        }

        [CreateProperty] [ShaderProperty(Description = "Blend factor")]
        public float BlendFactor {
            get => blendFactor;
            set => SetField(ref blendFactor, value, false);
        }

        [CreateProperty]
        public IEnumerable<SdfController> SdfControllers => GetFirstNestedControllers(transform);


        public override SdfData sdfData => Combinators.binaryCombine(
            MergeData,
            SdfControllers
                .Where(c => c != this)
                .Select(p => p.sdfData).ToArray()
        );

        static IEnumerable<SdfController> GetFirstNestedControllers(Transform root) {
            foreach (Transform child in root) {
                if (child.GetComponent<SdfController>() is { } controller)
                    yield return controller;
                else {
                    foreach (var c in GetFirstNestedControllers(child))
                        yield return c;
                }
            }
        }

        SdfData MergeData(SdfData d1, SdfData d2)
            => Operation switch
            {
                SIMPLE_UNION => Combinators.CombineWithExternalFunction("sdf::operators::unionSimple", d1, d2),
                INTERSECTION => Combinators.CombineWithExternalFunction("sdf::operators::intersectSimple", d1, d2),
                SMOOTH_UNION => Combinators.CombineWithExternalFunction(
                    "sdf::operators::unionSmooth",
                    d1,
                    d2,
                    (Identifier)SdfScene.sceneData.controllers[this].properties[blendFactorPropertyPath].identifier
                ),
                _ => throw new ArgumentOutOfRangeException(),
            };
    }
}
