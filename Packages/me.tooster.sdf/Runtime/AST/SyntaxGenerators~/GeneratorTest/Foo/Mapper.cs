using me.tooster.sdf.AST.Foo.Syntax;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Foo {
    public partial class Mapper<TState> where TState : MapperState, new() {
        Tree<foo>.Node? Visitor<Tree<foo>.Node>.Visit(Anchor<Syntax.Expressions.Affix.Succ> node) => throw new System.NotImplementedException();
        
        Tree<foo>.Node? Visitor<Tree<foo>.Node>.Visit(Anchor<Syntax.Expressions.Affix.Succ> a) {
            
            var newPlusToken = Visit(Anchor.New<Token<foo>>(a.Node.plusToken, a)) as PlusToken;
            var newZero = Visit(Anchor.New<Token<foo>>(a.Node.zero, a)) as ZeroLiteral;
            
            var newNode = new Syntax.Expressions.Affix.Succ {
                plusToken = newPlusToken, 
                zero = newZero
            };
            
            return a.Node == newNode ? a.Node : newNode;
        }
    }
}
