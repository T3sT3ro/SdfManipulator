#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Syntax {
    /// trivia list is a middleman between single trivia and token
    public record TriviaList<Lang> : Tree<Lang>.Node, IReadOnlyList<Trivia<Lang>> {
        private readonly IReadOnlyList<Trivia<Lang>> Trivias;
        
        public TriviaList() => Trivias = Array.Empty<Trivia<Lang>>();
        public TriviaList(IEnumerable<Trivia<Lang>> trivias) => Trivias = trivias.ToList();
        public TriviaList(params Trivia<Lang>[] trivias) => Trivias = trivias.ToList();


        public IEnumerator<Trivia<Lang>>                              GetEnumerator() => Trivias.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        public int                                                    Count           => Trivias.Count;
        public Trivia<Lang> this[int index] => Trivias[index];
        public TriviaList<Lang> Slice(int start, int length) => new(Trivias.Skip(start).Take(length));

        public static implicit operator TriviaList<Lang>(Trivia<Lang>[] trivias)     => new(trivias.AsEnumerable());
        public static implicit operator TriviaList<Lang>(List<Trivia<Lang>> trivias) => new(trivias.AsEnumerable());

        public override StringBuilder WriteTo(StringBuilder sb) {
            foreach (var trivia in Trivias)
                trivia.WriteTo(sb);
            return sb;
        }

        internal override void Accept(Visitor<Lang> visitor, Anchor a) => visitor.Visit((Anchor<TriviaList<Lang>>)a);
        internal override TR?  Accept<TR>(Visitor<Lang, TR> visitor, Anchor a) where TR : default => visitor.Visit((Anchor<TriviaList<Lang>>)a);
    }
}
