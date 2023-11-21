using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // something[index]
    [SyntaxNode] public partial record Indexer : Expression {
        public Expression        expression        { get; init; }
        public OpenBracketToken  openBracketToken  { get; init; } = new();
        public Expression        index             { get; init; }
        public CloseBracketToken closeBracketToken { get; init; } = new();
    }
}
