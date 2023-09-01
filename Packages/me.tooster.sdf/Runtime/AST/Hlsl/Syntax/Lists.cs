using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    // ( T , ... , T ) 
    public partial record ArgumentList<T> : Syntax<Hlsl> where T : Syntax<Hlsl> {
        private readonly OpenParenToken         /*_*/openParenToken;
        private readonly SeparatedList<Hlsl, T> /*_*/arguments;
        private readonly CloseParenToken        /*_*/closeParenToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openParenToken, arguments, closeParenToken };

        public static implicit operator ArgumentList<T>(T[] args) => new() { arguments = args.CommaSeparated() };

        public static implicit operator ArgumentList<T>(T singleton) =>
            new() { arguments = singleton.CommaSeparated() };
    }

    // { T , ... , T }
    public partial record BracedList<T> : Syntax<Hlsl> where T : Syntax<Hlsl> {
        private readonly OpenBraceToken         /*_*/openBraceToken;
        private readonly SeparatedList<Hlsl, T> /*_*/arguments;
        private readonly CloseBraceToken        /*_*/closeBraceToken;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openBraceToken, arguments, closeBraceToken };

        public static implicit operator BracedList<T>(T[] args) => new() { arguments = args.CommaSeparated() };
    }
}
