using System;
namespace me.tooster.sdf.AST.Syntax.CommonTrivia {
    public sealed record Whitespace<Lang> : SimpleTrivia<Lang> {
        public Whitespace(string text = " ") => Text = text;

        public override string Text {
            get => base.Text;
            init
                => base.Text = Extensions.WhitespaceRegex.IsMatch(value)
                    ? value
                    : throw new ArgumentException($"Whitespace text is invalid: \"{value}\"", nameof(value));
        }
    }
}
