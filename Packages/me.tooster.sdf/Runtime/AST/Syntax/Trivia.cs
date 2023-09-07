#nullable enable
using System.Text;

namespace me.tooster.sdf.AST.Syntax {
    // trivia list is a middleman between single trivia and token

    public abstract record Trivia<Lang> : Tree<Lang>.Node {
        public          TriviaList<Lang>? TriviaList { get; init; } // initialized by trivia list on assignment
        public override string            ToString() => base.ToString();

        protected override bool PrintMembers(StringBuilder builder) =>
            false; // to avoid stack overflow on TriviaList printing TriviaList
    }

    // should there even be something like unstructured trivia though?
    public abstract record SimpleTrivia<Lang> : Trivia<Lang> {
        public virtual string Text { get; init; }

        public override StringBuilder WriteTo(StringBuilder sb) => sb.Append(Text);
        public override string        ToString()                => base.ToString();
    }

    public abstract record StructuredTrivia<Lang> : Trivia<Lang> {
        public          Syntax<Lang>  Structure                 { get; init; }
        public override StringBuilder WriteTo(StringBuilder sb) => Structure.WriteTo(sb);
        public override string        ToString()                => base.ToString();
    }
}
