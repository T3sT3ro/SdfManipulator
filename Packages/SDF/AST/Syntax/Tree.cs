namespace AST.Syntax {
    public interface ITree { }
    
    public record Tree<Lang>(Syntax<Lang> Root) : ITree {
        public override string ToString()  => Root.BuildText();
    }
}