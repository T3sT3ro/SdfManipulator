#nullable enable
using System.Text;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Syntax {
    // trivia list is a middleman between single trivia and token

    public abstract record Trivia<Lang> : Tree<Lang>.Node;

    // should there even be something like unstructured trivia though?
    public abstract record SimpleTrivia<Lang> : Trivia<Lang> {
        public virtual string Text { get; init; }

        public override StringBuilder WriteTo(StringBuilder sb) => sb.Append(Text);
    }

    public abstract record StructuredTrivia<Lang> : Trivia<Lang> {
        public          Syntax<Lang>  Structure                 { get; init; }
        public override StringBuilder WriteTo(StringBuilder sb) => Structure.WriteTo(sb);
    }
}
