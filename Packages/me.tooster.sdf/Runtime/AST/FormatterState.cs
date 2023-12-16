#nullable enable
using System;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST {
    public record FormatterState : MapperState {
        private string       SingleIndent       { get; init; } = "    ";
        private int          CurrentIndentLevel { get; set; }  = 0;
        public  Lazy<string> CurrentIndent      { get; private set; }
        private bool         NewLineStarts      { get; set; } = false;

        private static string RepeatString(string s, uint n) => string.Concat(Enumerable.Repeat(s, (int)n));

        public FormatterState() {
            CurrentIndent = new Lazy<string>(() => RepeatString(SingleIndent, (uint)CurrentIndentLevel));
        }

        public void Indent(int n = 1) {
            CurrentIndentLevel += n;
            CurrentIndent = new Lazy<string>(() => RepeatString(SingleIndent, (uint)CurrentIndentLevel));
        }

        public void Deindent(int n = 1) {
            CurrentIndentLevel = Math.Max(0, CurrentIndentLevel - n);
            CurrentIndent = new Lazy<string>(() => RepeatString(SingleIndent, (uint)CurrentIndentLevel));
        }

        public void MarkLineEnd() { NewLineStarts = true; }

        public bool PollLineStart(out string? indent) {
            indent = NewLineStarts ? CurrentIndent.Value : null;
            NewLineStarts = false;
            return indent is not null;
        }
    }
}
