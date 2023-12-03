#nullable enable
using System;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST {
    public record FormatterOptions {
        public char indentCharacter = ' ';
        public int  indentWidth     = 4;
    }

    public abstract class Formatter<Lang, TOpts> : Mapper<Lang> where TOpts : FormatterOptions, new() {
        protected Formatter(TOpts? options = null, bool descentIntoTrivia = true, int startingIndent = 0) :
            base(descentIntoTrivia) {
            this.options = options ?? new();
            currentIndent = startingIndent;
            currentIndentString = new string(this.options.indentCharacter, currentIndent * this.options.indentWidth);
        }

        private TOpts  options;
        private int    currentIndent;
        private string currentIndentString;

        protected void Indent(int n = 1) {
            currentIndent += n;
            currentIndentString = new string(options.indentCharacter, currentIndent * options.indentWidth);
        }

        protected void Deindent(int n = 1) {
            currentIndent = Math.Max(0, currentIndent - n);
            currentIndentString = new string(options.indentCharacter, currentIndent * options.indentWidth);
        }
    }
}
