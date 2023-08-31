using System.Text.RegularExpressions;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Trivias {
    public partial record Whitespace : Trivia<Hlsl> {
        private static Regex whitespaceRegex = new(@"\S+", RegexOptions.Compiled);
    }
}
