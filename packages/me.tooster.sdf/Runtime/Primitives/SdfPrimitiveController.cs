using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.Editor.Controllers.Data;
using UnityEngine;
using Type = System.Type;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    public abstract class SdfPrimitiveController : Controller, IModifier<VectorData, ScalarData> {
        public abstract ScalarData Apply(VectorData input, Processor processor);


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
