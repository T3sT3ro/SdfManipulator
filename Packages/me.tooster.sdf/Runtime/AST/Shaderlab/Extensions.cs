using System.Collections.Generic;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
namespace me.tooster.sdf.AST.Shaderlab {
    public static class Extensions {
        public static SyntaxList<shaderlab, T> ToSyntaxList<T>(this IEnumerable<T> args) where T : Syntax<shaderlab> => new(args);

        public static ArgumentList<T> ToArgumentList<T>(this IEnumerable<T> args) where T : Syntax<shaderlab>
            => new() { arguments = args.CommaSeparated() };

        public static SeparatedList<shaderlab, T> CommaSeparated<T>(this IEnumerable<T> nodes)
            where T : Syntax<shaderlab>
            => SeparatedList<shaderlab, T>.WithSeparator<CommaToken>(nodes);

        public static SeparatedList<shaderlab, T> CommaSeparated<T>(this T singleton)
            where T : Syntax<shaderlab>
            => SeparatedList<shaderlab, T>.WithSeparator<CommaToken>(singleton);
    }
}
