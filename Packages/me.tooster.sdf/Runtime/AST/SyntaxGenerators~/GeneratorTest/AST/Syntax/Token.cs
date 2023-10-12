#nullable enable
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace me.tooster.sdf.AST.Syntax {
    public abstract record Token<Lang> : SyntaxOrToken<Lang> {
        public TriviaList<Lang>? leadingTriviaList { get; init; }
        public TriviaList<Lang>? trailingTriviaList { get; init; }
        public abstract  string            Text { get; }

        public override StringBuilder WriteTo(StringBuilder sb) {
            leadingTriviaList?.WriteTo(sb);
            sb.Append(Text);
            trailingTriviaList?.WriteTo(sb);
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
