using System;
using System.Text.RegularExpressions;

namespace me.tooster.sdf.AST.Syntax.CommonTrivia {
    public sealed record Whitespace<Lang> : SimpleTrivia<Lang> {
        public override string Text {
            get => base.Text;
            init => base.Text = Extensions.WhitespaceRegex.IsMatch(value)
                ? value
                : throw new ArgumentException($"Whitespace text is invalid: \"{value}\"", nameof(value));
        }

        public Whitespace(string text = " ") { Text = text; }
    }
}
