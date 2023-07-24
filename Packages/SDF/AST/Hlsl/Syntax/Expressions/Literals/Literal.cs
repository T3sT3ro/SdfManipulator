#nullable enable
using System;
using System.Text.RegularExpressions;

namespace AST.Hlsl.Syntax.Expressions.Literals {
    public abstract record Literal : Expression {
        public HlslToken Token { get; internal set; } 
    }
}
