#nullable enable
namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Call : Expression {
        public IdentifierName   id        { get; set; }
        public Expression[] arguments { get; set; }
    }
}
