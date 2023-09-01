#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace me.tooster.sdf.AST.Syntax {
    // trivia list is a middleman between single trivia and token
    public record TriviaList<Lang> : IReadOnlyList<Trivia<Lang>>, IWriteable {
        public           Token<Lang>?                Token { get; init; } // initialized on creation by token
        private readonly IReadOnlyList<Trivia<Lang>> Trivias;

        public TriviaList() { Trivias = Array.Empty<Trivia<Lang>>(); }

        public TriviaList(IEnumerable<Trivia<Lang>> trivias) {
            Trivias = trivias.Select(trivia => trivia with { TriviaList = this }).ToList();
        }

        public IEnumerator<Trivia<Lang>>                              GetEnumerator() => Trivias.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        public int                                                    Count           => Trivias.Count;
        public Trivia<Lang> this[int      index] => Trivias[index];
        public TriviaList<Lang> Slice(int start, int length) => new(Trivias.Skip(start).Take(length));

        public static implicit operator TriviaList<Lang>(Trivia<Lang>[]     trivias) => new(trivias.AsEnumerable());
        public static implicit operator TriviaList<Lang>(List<Trivia<Lang>> trivias) => new(trivias.AsEnumerable());

        public StringBuilder WriteTo(StringBuilder sb) {
            foreach (var trivia in Trivias)
                trivia.WriteTo(sb);
            return sb;
        }
    }

    public abstract record AbstractTrivia<Lang> : IWriteable {
        public          TriviaList<Lang>? TriviaList { get; init; } // initialized by trivia list on assignment
        public abstract StringBuilder     WriteTo(StringBuilder sb);
        public override string            ToString() => WriteTo(new StringBuilder()).ToString();
    }

    // should there even be something like unstructured trivia though?
    public abstract record Trivia<Lang> : AbstractTrivia<Lang> {
        public virtual string Text { get; init; } = "";
        

        public override StringBuilder WriteTo(StringBuilder sb) => sb.Append(Text);
        public override string        ToString()                => base.ToString();
    }

    public abstract record StructuredTrivia<Lang> : AbstractTrivia<Lang> {
        public          Syntax<Lang> Structure  { get; init; }
        public override string       ToString() => base.ToString();
    }
}
