#nullable enable
using System;
using System.Text.RegularExpressions;
namespace me.tooster.sdf.AST.Syntax.CommonSyntax {
    public abstract record ValidatedToken<Lang> : Token<Lang> {
        readonly string rawText = "";

        public override string Text => rawText;

        protected abstract Regex Pattern { get; }

        /// <summary>Creates new token by validating the text.</summary>
        /// <exception cref="ArgumentException">if text doesn't match pattern</exception>
        public virtual string ValidatedText {
            init {
                if (!Pattern.IsMatch(value)) {
                    throw new ArgumentException(
                        $"The provided text doen't match token's required pattern.\npattern: {Pattern}\ntext: '{value}'"
                    );
                }

                rawText = value;
            }
        }

        // for internal usage to bypass regex match with direct value assignment
        protected string TextUnsafe {
            init => rawText = value;
        }

        public override string ToString() => base.ToString();
    }



    public abstract record Literal<Lang> : ValidatedToken<Lang> {
        public override string ToString() => base.ToString();
    }



    public record EofToken<Lang> : Token<Lang> {
        public override string Text       => "";
        public override string ToString() => base.ToString();
    }



    public record InjectedLanguageStartToken<Lang> : Token<Lang> {
        public override string Text       => "";
        public override string ToString() => base.ToString();
    }



    public record InjectedLanguageEndToken<Lang> : Token<Lang> {
        public override string Text       => "";
        public override string ToString() => base.ToString();
    }
}
