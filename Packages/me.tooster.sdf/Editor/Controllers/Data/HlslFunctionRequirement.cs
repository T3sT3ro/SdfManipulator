using System;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Definitions;
namespace me.tooster.sdf.Editor.Controllers.Data {
    [Serializable]
    public record HlslFunctionRequirement : API.Data.Requirement {
        public Identifier         functionIdentifier { get; init; }
        public FunctionDefinition requiredFunction   { get; init; }
    }
}
