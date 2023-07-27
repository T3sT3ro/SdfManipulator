using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl {
    public static class Extensions {
        // todo: parethesize by default, simplify with rules under associativity and precedence
        
        public static Expression ParenthesizeFor(this Expression child, Expression parent) =>
            child.Precedence() > parent.Precedence() ? Parenthesized.From(child) : child;
        
    }
}
