#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace AST.Syntax {
    public abstract record Trivia<Lang> {
        public         TriviaList<Lang> TriviaList { get; init; }
        public virtual string           Text       { get; init; }
    }

    public record TriviaList<Lang> : IReadOnlyList<Trivia<Lang>> {
        public           Token<Lang>                 Token { get; init; }
        private readonly IReadOnlyList<Trivia<Lang>> Trivias;

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
    }
}
