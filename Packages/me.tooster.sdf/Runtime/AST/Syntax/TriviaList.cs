#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace me.tooster.sdf.AST.Syntax {
    /// trivia list is a middleman between single trivia and token
    public record TriviaList<Lang> : Tree<Lang>.Node, IReadOnlyList<Trivia<Lang>> {
        readonly IReadOnlyList<Trivia<Lang>> Trivias = Array.Empty<Trivia<Lang>>();

        public TriviaList() { }
        public TriviaList(IEnumerable<Trivia<Lang>> trivias) => Trivias = trivias.ToList();
        public TriviaList(params Trivia<Lang>[] trivias) => Trivias = trivias.ToList();

        public static readonly TriviaList<Lang> Empty = new();

        public IEnumerator<Trivia<Lang>>                              GetEnumerator() => Trivias.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        public int                                                    Count           => Trivias.Count;
        public Trivia<Lang> this[int index] => Trivias[index];
        public TriviaList<Lang> Slice(int start, int length) => new(Trivias.Skip(start).Take(length));

        /// See <see cref="Extensions.Splice{T}(IEnumerable{T}, int, int, IEnumerable{T})"/>
        public TriviaList<Lang> Splice(int index, int deleteCount, IEnumerable<Trivia<Lang>> elements)
            => new(Trivias.Splice(index, deleteCount, elements));

        /// See <see cref="Extensions.Splice{T}(IEnumerable{T}, int, int, IEnumerable{T})"/>
        public TriviaList<Lang> Splice(int index, int deleteCount, params Trivia<Lang>[] elements)
            => new(Trivias.Splice(index, deleteCount, elements));

        public static implicit operator TriviaList<Lang>(Trivia<Lang>[] trivias)     => new(trivias.AsEnumerable());
        public static implicit operator TriviaList<Lang>(List<Trivia<Lang>> trivias) => new(trivias.AsEnumerable());
        public static implicit operator TriviaList<Lang>(Trivia<Lang> trivia)        => new(trivia);

        public override StringBuilder WriteTo(StringBuilder sb) {
            foreach (var trivia in Trivias)
                trivia.WriteTo(sb);
            return sb;
        }

        internal override void Accept(Visitor<Lang> visitor, Anchor? parent) => visitor.Visit(Anchor.New(this, parent));

        internal override TR? Accept<TR>(Visitor<Lang, TR> visitor, Anchor? parent) where TR : default
            => visitor.Visit(Anchor.New(this, parent));

        public override string ToString() => WriteTo(new StringBuilder()).ToString();
    }
}
