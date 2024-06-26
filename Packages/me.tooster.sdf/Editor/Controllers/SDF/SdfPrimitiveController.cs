using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.Editor.Controllers.Data;
using UnityEditor;
using UnityEngine;
using Type = System.Type;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    public abstract class SdfPrimitiveController : Controller, IModifier<VectorData, ScalarData> {
        public abstract ScalarData Apply(VectorData input, Processor processor);

        public static void InstantiatePrimitive<TController>(string name) where TController : SdfPrimitiveController, new() {
            Undo.IncrementCurrentGroup();

            var sdf = new GameObject(name);

            Undo.RegisterCreatedObjectUndo(sdf, $"Create {name}");

            ObjectNames.SetNameSmart(sdf, name);
            var target = Selection.activeGameObject;

            var transformController = sdf.AddComponent<TransformController>();
            var primitiveSdfController = sdf.AddComponent<TController>();
            var sdfObjectController = sdf.AddComponent<SdfController>();

            sdfObjectController.sdfModifiers = new Controller[] { transformController, primitiveSdfController };

            // call this after adding components to properly filter in OnTransformChildrenChange
            if (target) sdf.transform.SetParent(Selection.activeTransform);
            sdf.transform.localPosition = Vector3.zero;

            Undo.RegisterCompleteObjectUndo(sdf, $"Create {name}");

            Selection.activeGameObject = sdf;

            Undo.SetCurrentGroupName($"Create a primitive SDF {name}");
        }


        public override IData Apply(IData input, Processor processor) => Apply((VectorData)input, processor);
        public override Type  GetInputType()                          => typeof(VectorData);
        public override Type  GetOutputType()                         => typeof(ScalarData);

        public static FunctionDefinition createSdfPrimitiveFunction(Identifier identifier, Block functionBody)
            => new()
            {
                returnType = (AST.Hlsl.Syntax.Type.Predefined)Constants.ScalarKind.@float,
                id = identifier,
                paramList = new Parameter { type = SdfData.pData.typeSyntax, id = SdfData.pParamName },
                body = functionBody,
            };
    }
}
