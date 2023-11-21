#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Syntax {
    public record TriviaList<Lang> : Tree<Lang>.Node, IReadOnlyList<Trivia<Lang>> {
        private readonly IReadOnlyList<Trivia<Lang>> Trivias;
        
        public TriviaList() => Trivias = Array.Empty<Trivia<Lang>>();
        public TriviaList(IEnumerable<Trivia<Lang>> trivias) => Trivias = trivias.ToList();


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
    }
}
