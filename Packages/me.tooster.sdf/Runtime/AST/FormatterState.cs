#nullable enable
using System;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST {
    public record FormatterState : MapperState {
        public char indentCharacter = ' ';
        public int  indentWidth     = 4;
        public int  currentIndent   = 0;
        public string currentIndentString = "";


        public void Indent(int n = 1) {
            currentIndent += n;
            currentIndentString = new string(indentCharacter, currentIndent * indentWidth);
        }

        public void Deindent(int n = 1) {
            currentIndent = Math.Max(0, currentIndent - n);
            currentIndentString = new string(indentCharacter, currentIndent * indentWidth);
        }
    }
}
