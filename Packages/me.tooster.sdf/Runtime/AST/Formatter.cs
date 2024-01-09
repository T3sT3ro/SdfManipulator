#nullable enable
using System;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST {
    /// <summary>
    /// Mutable state of the formatter to support line breaks and indents
    /// </summary>
    public record FormatterState : MapperState {
        private string SingleIndent { get; init; } = "    ";
        private int    currentIndentLevel = 0;

        public int CurrentIndentLevel {
            get => currentIndentLevel;
            set {
                currentIndentLevel = Math.Max(0, value);
                CurrentIndent = new Lazy<string>(() => RepeatString(SingleIndent, (uint)CurrentIndentLevel));
            }
        }

        public  Lazy<string> CurrentIndent { get; private set; }
        private bool         NewLineStarts { get; set; } = false;

        private static string RepeatString(string s, uint n) => string.Concat(Enumerable.Repeat(s, (int)n));

        public FormatterState() {
            CurrentIndent = new Lazy<string>(() => RepeatString(SingleIndent, (uint)CurrentIndentLevel));
        }

        public void MarkLineEnd() { NewLineStarts = true; }

        public bool PollLineStart(out string? indent) {
            indent = NewLineStarts ? CurrentIndent.Value : null;
            NewLineStarts = false;
            return indent is not null;
        }
    }

    public interface Formatter<Lang> : Visitor<Lang, Tree<Lang>.Node> {
        
        FormatterState State { get; }
        
        internal int  getIndentChange<T>(Anchor<T> a) where T : Token<Lang>;
        internal bool breakLineAfter<T>(Anchor<T> a) where T : Token<Lang>;
        
        Tree<Lang>.Node? Visitor<Lang, Tree<Lang>.Node>.Visit(Anchor<Token<Lang>> a) {
            return a.Node;
        }
    }
}
