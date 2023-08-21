using System.Collections.Generic;
using AST.Hlsl.Syntax;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl {
    public static class Extensions {
        // todo: parethesize by default, simplify with rules under associativity and precedence
        public static Expression ParenthesizeFor(this Expression child, Expression parent) =>
            child.Precedence() > parent.Precedence() ? new Parenthesized { expression = child } : child;

        public static BracketInitializerList<T> ToBracketInitializerList<T>(this IEnumerable<T> syntaxList)
            where T : HlslSyntax =>
            new BracketInitializerList<T>(syntaxList);
    }
}
