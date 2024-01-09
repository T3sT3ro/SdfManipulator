using System.Text.RegularExpressions;

namespace me.tooster.sdf.AST.Syntax.CommonTrivia {
    public sealed record Whitespace<Lang> : SimpleTrivia<Lang> {
        public override string Text {
            get => base.Text;
            init => base.Text = Extensions.WhitespaceRegex.IsMatch(value) ? value : " ";
        }
        
        public Whitespace(string text = " ") { Text = text; }
    }
}
