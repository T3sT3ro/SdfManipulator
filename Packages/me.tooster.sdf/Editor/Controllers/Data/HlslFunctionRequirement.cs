using System;
using me.tooster.sdf.AST.Hlsl.Syntax;
using FunctionDefinition = me.tooster.sdf.AST.Hlsl.Syntax.Statements.FunctionDefinition;
namespace me.tooster.sdf.Editor.Controllers.Data {
    [Serializable]
    public record HlslFunctionRequirement : API.Data.Requirement {
        public Identifier         functionIdentifier { get; init; }
        public FunctionDefinition functionDefinition { get; init; }
    }
}
