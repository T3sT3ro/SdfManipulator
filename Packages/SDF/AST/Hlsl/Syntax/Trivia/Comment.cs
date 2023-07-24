namespace AST.Hlsl.Syntax.Trivia {
    public abstract record Comment : HlslTrivia {
        public record Whitespace : Trivia;

        public abstract record Comment : Trivia {
            public record Line : Comment;
            public record Block : Comment;
        }

        public void WriteTo(TextWriter writer) { writer.Write(Text); }
    }
}
