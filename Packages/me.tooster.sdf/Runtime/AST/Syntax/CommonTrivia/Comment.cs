namespace me.tooster.sdf.AST.Syntax.CommonTrivia {
    public abstract record Comment<Lang> : SimpleTrivia<Lang> {
        public record Line<Lang> : Comment<Lang> {
            public override string Text { get => base.Text; init => base.Text = $"// {value}"; }
        }

        public record Block<Lang> : Comment<Lang> {
            public override string Text { get => base.Text; init => base.Text = $"/*\n{value}\n*/"; }
        }
    }
}
