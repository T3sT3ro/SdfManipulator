using System;
using me.tooster.sdf.AST.Hlsl.Syntax;
namespace me.tooster.sdf.Editor.Controllers.Data {
    [Serializable]
    public record HlslUniformRequirement : API.Data.Requirement {
        public Identifier SymbolId { get; init; }
    }
}
