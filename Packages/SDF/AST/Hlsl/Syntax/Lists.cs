using System;
using System.Collections.Generic;
using System.Linq;
using AST.Hlsl.Syntax.Expressions;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    // ( T , ... , T ) 
    public partial record ArgumentList<T> : Syntax<Hlsl> where T : Syntax<Hlsl> {
        private readonly OpenParenToken         _openParenToken;
        private readonly SeparatedList<Hlsl, T> _arguments;
        private readonly CloseParenToken        _closeParenToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openParenToken, arguments, closeParenToken };

        public static implicit operator ArgumentList<T>(T[] args) => new() { arguments = args.CommaSeparated() };

        public static implicit operator ArgumentList<T>(T singleton) =>
            new() { arguments = singleton.CommaSeparated() };
    }

    // { T , ... , T }
    public partial record BracedList<T> : Syntax<Hlsl> where T : Syntax<Hlsl> {
        private readonly OpenBraceToken         _openBraceToken;
        private readonly SeparatedList<Hlsl, T> _arguments;
        private readonly CloseBraceToken        _closeBraceToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openBraceToken, arguments, closeBraceToken };

        public static implicit operator BracedList<T>(T[] args) => new() { arguments = args.CommaSeparated() };
    }
}
