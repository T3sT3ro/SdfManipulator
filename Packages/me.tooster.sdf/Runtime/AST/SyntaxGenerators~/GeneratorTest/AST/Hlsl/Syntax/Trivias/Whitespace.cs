using System;
using System.Text.RegularExpressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Trivias {
    public partial record Whitespace : Trivia<Hlsl> {
        private static Regex whitespaceRegex = new(@"\s+", RegexOptions.Compiled);

        public override string Text {
            get => base.Text;
            init => base.Text = whitespaceRegex.IsMatch(value) ? value : " ";
        }
    }
}
