using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Trivias {
    public abstract partial record Comment : SimpleTrivia<hlsl> {
        public record Line : Comment {
            public override string Text { get => base.Text; init => base.Text = $"/*\n{value}\n*/"; }
        }

        public record Block : Comment {
            public override string Text { get => base.Text; init => base.Text = $"// {value}"; }
        }
    }
}
