#nullable enable
using System.Text.RegularExpressions;

namespace AST.Hlsl.Syntax.Expressions.Literals {
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-appendix-grammar">grammar</a>
    public record Float : Literal {
        protected override Regex pattern => new(@"^((\d*\.\d+|\d+\.\d*)([eE][+-]?\d+)?|\d+([eE][+-]?\d+))[hHfFlL]?$");
    }
}
