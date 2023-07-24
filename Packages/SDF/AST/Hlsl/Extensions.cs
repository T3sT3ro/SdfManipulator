using System;
using AST.Hlsl.Syntax;
using AST.Hlsl.Syntax.Expressions;
using AST.Hlsl.Syntax.Expressions.Literals;
using AST.Hlsl.Syntax.Expressions.Operators;
using Boolean = AST.Hlsl.Syntax.Expressions.Literals.Boolean;
using Decimal = AST.Hlsl.Syntax.Expressions.Literals.Decimal;

namespace AST.Hlsl {
    public static class Extensions {
        // todo: parethesize by default, simplify with rules under associativity and precedence
        
        public static Expression ParenthesizeFor(this Expression child, Expression parent) =>
            child.Precedence() > parent.Precedence() ? Parenthesized.From(child) : child;
        
    }
}
