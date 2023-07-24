#nullable enable
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    public record VariableDeclaration : Statement, For.Initializer {
        public enum Storage {
            EXTERN, NOINTERPOLATION, PRECISE, SHARED, GROUPSHARED, STATIC, UNIFORM
        }

        public enum TypeModifier {
            CONST, ROW_MAJOR, COLUMN_MAJOR
        }

        public Storage?      storage      { get; set; }
        public TypeModifier? typeModifier { get; set; }
        public Type.Type     type         { get; set; }
        public IdentifierName    id           { get; set; }
        public uint[]?       arraySizes   { get; set; }
        public Semantic?     semantic     { get; set; }
        public Expression?   initializer  { get; set; }
    }
}