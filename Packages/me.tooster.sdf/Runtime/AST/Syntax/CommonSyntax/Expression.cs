using System.Text;

namespace me.tooster.sdf.AST.Syntax.CommonSyntax {
    public abstract partial record Expression<Lang> : Syntax<Lang> {
        internal override void Accept(Visitor<Lang> visitor, Anchor parent) 
            => visitor.Visit(Anchor.New(this, parent));

        internal override TR Accept<TR>(Visitor<Lang, TR> visitor, Anchor parent) where TR : default =>
            visitor.Visit(Anchor.New(this, parent));

        public override string ToString() => WriteTo(new StringBuilder()).ToString();
    }
}
