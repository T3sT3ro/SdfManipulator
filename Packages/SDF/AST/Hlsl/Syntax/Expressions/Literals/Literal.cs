#nullable enable
using System;
using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Literals {
    public abstract record Literal : Expression {
        public          HlslToken                        LiteralToken        { get; internal set; }
        public override IReadOnlyList<HlslSyntax>        ChildNodes          => Array.Empty<HlslSyntax>();
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens { get; }


        /// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-appendix-grammar">grammar</a>
        public record Decimal : Literal;

        /// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-appendix-grammar">grammar</a>
        public record Float : Literal;

        public record Boolean : Literal;
    }
}
