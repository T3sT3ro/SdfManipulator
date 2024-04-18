using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
namespace me.tooster.sdf.Editor.Controllers.Data {
    public record ScalarData {
        /// returns a HLSL's TypeSyntax node representing this vector type
        public readonly Type typeSyntax;

        public ScalarData() => typeSyntax = (Type.Predefined)scalarType;
        public Constants.ScalarKind scalarType           { get; init; }
        public Expression<hlsl>     evaluationExpression { get; init; }
    }
}
