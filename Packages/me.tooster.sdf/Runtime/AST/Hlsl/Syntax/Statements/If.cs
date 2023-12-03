#nullable enable
namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // if (test) then
    // if (test) then else elseStatement
    [SyntaxNode] public partial record If : Statement {
        public IfKeyword       ifKeyword  { get; init; } = new();
        public OpenParenToken  openParen  { get; init; } = new();
        public Expression      test       { get; init; }
        public CloseParenToken closeParen { get; init; } = new();
        public Statement       then       { get; init; }
        public ElseClause?     elseClause { get; init; }
    }
}
