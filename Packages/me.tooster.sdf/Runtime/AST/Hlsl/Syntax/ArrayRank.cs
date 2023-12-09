using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    [SyntaxNode] public partial record ArrayRank {
        public OpenBracketToken  openBracketToken  { get; init; } = new();
        public LiteralExpression dimension         { get; init; }
        public CloseBracketToken closeBracketToken { get; init; } = new();

        internal override void Accept(AST.Visitor<hlsl> visitor, Anchor a)                        => ((Visitor)visitor).Visit((Anchor<ArrayRank>)a); 
        internal override TR?  Accept<TR>(Visitor<hlsl, TR> visitor, Anchor a) where TR : default => ((Visitor<TR>)visitor).Visit((Anchor<ArrayRank>)a);
    }
}
