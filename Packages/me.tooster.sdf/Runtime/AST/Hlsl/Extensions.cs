using System.Collections.Generic;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl {
    public static class Extensions {
        // todo: parenthesize by default, simplify with rules under associativity and precedence
        public static Expression ParenthesizeFor(this Expression child, Expression parent) =>
            child.Precedence() > parent.Precedence() ? new Parenthesized { expression = child } : child;

        // convert enumerables to appropriate lists
        public static SyntaxList<hlsl, T> ToSyntaxList<T>(this IEnumerable<T> args)
            where T : Syntax<hlsl> => new SyntaxList<hlsl, T>(args);

        public static ArgumentList<T> ToArgumentList<T>(this IEnumerable<T> args)
            where T : Syntax<hlsl> => new ArgumentList<T> { arguments = args.CommaSeparated() };

        public static BracedList<T> ToBracedList<T>(this IEnumerable<T> args)
            where T : Syntax<hlsl> => new BracedList<T> { arguments = args.CommaSeparated() };


        public static SeparatedList<hlsl, T> CommaSeparated<T>(this IEnumerable<T> nodes)
            where T : Syntax<hlsl> => SeparatedList<hlsl, T>.WithSeparator<CommaToken>(nodes);

        public static SeparatedList<hlsl, T> CommaSeparated<T>(this T singleton)
            where T : Syntax<hlsl> => SeparatedList<hlsl, T>.WithSeparator<CommaToken>(singleton);
    }
}
