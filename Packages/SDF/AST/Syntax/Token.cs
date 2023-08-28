using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AST.Syntax {
    public abstract record Token<Lang> : SyntaxOrToken<Lang> {
        private readonly TriviaList<Lang> _leadingTriviaList  = Array.Empty<Trivia<Lang>>();
        private readonly TriviaList<Lang> _trailingTriviaList = Array.Empty<Trivia<Lang>>();
        public abstract  string           Text { get; }

        public TriviaList<Lang> LeadingTriviaList {
            get => _leadingTriviaList;
            init => _leadingTriviaList = new(value with { Token = this });
        }

        public TriviaList<Lang> TrailingTriviaList {
            get => _trailingTriviaList;
            init => _trailingTriviaList = new(value with { Token = this });
        }

        public override void WriteTo(StringBuilder sb) {
            foreach (var leading in LeadingTriviaList) sb.Append(leading);
            sb.Append(Text);
            foreach (var trailing in LeadingTriviaList) sb.Append(trailing);
        }
    }
}
