#nullable enable
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace me.tooster.sdf.AST.Syntax {
    public abstract record Token<Lang> : SyntaxOrToken<Lang> {
        private readonly TriviaList<Lang>? _leadingTriviaList;
        private readonly TriviaList<Lang>? _trailingTriviaList;
        public abstract  string            Text { get; }

        public TriviaList<Lang>? LeadingTriviaList {
            get => _leadingTriviaList;
            init => _leadingTriviaList = value == null ? null : new(value with { Token = this });
        }

        public TriviaList<Lang>? TrailingTriviaList {
            get => _trailingTriviaList;
            init => _trailingTriviaList = value == null ? null : new(value with { Token = this });
        }

        public override StringBuilder WriteTo(StringBuilder sb) {
            LeadingTriviaList?.WriteTo(sb);
            sb.Append(Text);
            TrailingTriviaList?.WriteTo(sb);
            return sb;
        }

        public override string ToString() => base.ToString();
    }

    public abstract record ValidatedToken<Lang> : Token<Lang> {
        private readonly string validatedText;

        public override string Text => validatedText;

        protected abstract Regex Pattern { get; }

        /// <summary>Creates new token by validating the text.</summary>
        /// <exception cref="ArgumentException">if text doesn't match pattern</exception>
        public virtual string ValidatedText {
            init {
                if (!Pattern.IsMatch(value))
                    throw new ArgumentException($"Token text: {value} doesn't match pattern: {Pattern}");

                validatedText = value;
            }
        }

        protected string TextUnsafe {
            init => validatedText = value;
        }
    }
}
