using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    // ( T , ... , T ) 
    [Syntax] public partial record ArgumentList<T> : Syntax<Hlsl> where T : Syntax<Hlsl> {
        [Init] private readonly OpenParenToken         _openParenToken;
        [Init] private readonly SeparatedList<Hlsl, T> _arguments;
        [Init] private readonly CloseParenToken        _closeParenToken;

        public static implicit operator ArgumentList<T>(T[] args) => new() { arguments = args.CommaSeparated() };

        public static implicit operator ArgumentList<T>(T singleton) =>
            new() { arguments = singleton.CommaSeparated() };
    }

    // { T , ... , T }
    [Syntax] public partial record BracedList<T> : Syntax<Hlsl> where T : Syntax<Hlsl> {
        [Init] private readonly OpenBraceToken         _openBraceToken;
        [Init] private readonly SeparatedList<Hlsl, T> _arguments;
        [Init] private readonly CloseBraceToken        _closeBraceToken;

        public static implicit operator BracedList<T>(T[] args)    => new() { arguments = args.CommaSeparated() };
        public static implicit operator BracedList<T>(T singleton) => new() { arguments = singleton.CommaSeparated() };
    }
}
