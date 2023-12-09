#nullable enable
using System.Text;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Syntax {
    /**
     * Trivia represents a piece of text that is not part of the syntax tree but is still important in the code, i.e.
     * whitespace, comment etc.
     * The general rule for trivia ownership is that a token owns any trivia after it on the same line
     * up to the next token. Any trivia after that line is associated with the following token. The first token in
     * the source file gets all the initial trivia
     * https://github.com/dotnet/roslyn/blob/main/docs/wiki/Roslyn-Overview.md#syntax-trivia
     * */
    public abstract record Trivia<Lang> : Tree<Lang>.Node;

    // should there even be something like unstructured trivia though?
    public abstract record SimpleTrivia<Lang> : Trivia<Lang> {
        public virtual string Text { get; init; }

        public override StringBuilder WriteTo(StringBuilder sb) => sb.Append(Text);
        
        internal override void Accept(Visitor<Lang> visitor, Anchor parent) => visitor.Visit(Anchor.New(this, parent));
        internal override TR?  Accept<TR>(Visitor<Lang, TR> visitor, Anchor parent) where TR : default => visitor.Visit(Anchor.New(this, parent));
    }

    public abstract record StructuredTrivia<Lang, T> : Trivia<Lang> where T : SyntaxOrToken<Lang> {
        public          T?            Structure                 { get; init; }
        public override StringBuilder WriteTo(StringBuilder sb) => Structure?.WriteTo(sb) ?? sb;
        
        internal override void Accept(Visitor<Lang> visitor, Anchor parent) => visitor.Visit(Anchor.New(this, parent));
        internal override TR?  Accept<TR>(Visitor<Lang, TR> visitor, Anchor parent) where TR : default => visitor.Visit(Anchor.New(this, parent));
    }
}
