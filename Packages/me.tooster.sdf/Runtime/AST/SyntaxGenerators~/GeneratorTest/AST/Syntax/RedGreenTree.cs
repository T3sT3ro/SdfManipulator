#nullable enable
namespace me.tooster.sdf.AST.Syntax {
    // source: https://blog.yaakov.online/red-green-trees/
    // sealed class RedGreenTree<T> {
    //     readonly GreenNode<T> rootNode;
    //     public RedGreenTree(GreenNode<T> rootNode) { this.rootNode = rootNode; }
    //     public RedNode<T> RootNode => new RedNode<T>(rootNode, null);
    // }
    //
    // public class GreenNode {
    //     public IEnumerable<GreenNode> Children { get; }
    //     public GreenNode(IEnumerable<GreenNode> children) { Children = children; }
    // }
    //
    // /// top-down tree node wrapper, immutable
    // public sealed class GreenNode<T> {
    //     public T                         Value    { get; }
    //     public IEnumerable<GreenNode<T>> Children { get; }
    //
    //     public GreenNode(T value, IEnumerable<GreenNode<T>> children) {
    //         Value = value;
    //         Children = children;
    //     }
    // }
    //
    // /// readonly, bottom-up view of the tree, created dynamically during traversal
    // public sealed class RedNode<T> {
    //     public   RedNode<T>   Parent { get; }
    //     readonly GreenNode<T> value;
    //     public   T            Value => value.Value;
    //
    //     public RedNode(GreenNode<T> value, RedNode<T> parent) {
    //         this.value = value;
    //         Parent = parent;
    //     }
    //
    //     public IEnumerable<RedNode<T>> Children =>
    //         value.Children?.Select(c => new RedNode<T>(c, this)) ?? Enumerable.Empty<RedNode<T>>();
    // }

    // --------- actual impl 
    // TODO: use source generators to add the red-green-like tree from Roslyn, auto-generated from annotations
    // Instantiating would be possible only with https://stackoverflow.com/questions/8718199/passing-a-type-to-a-generic-constructor

    public abstract class RedNode {
        public RedNode? ParentSyntaxTriviaList { get; }
        protected RedNode(RedNode? parentSyntaxTriviaList) => ParentSyntaxTriviaList = parentSyntaxTriviaList;
    }

    public class RedSyntax<Lang> : RedNode {
        public Syntax<Lang> GreenSyntax { get; }

        protected RedSyntax(Syntax<Lang> greenSyntax, RedNode? parentSyntax) : base(parentSyntax) =>
            GreenSyntax = greenSyntax;
    }

    public class RedToken<Lang> : RedNode {
        public Token<Lang> GreenToken { get; }

        protected RedToken(Token<Lang> greenToken, RedSyntax<Lang>? parentSyntax) : base(parentSyntax) =>
            GreenToken = greenToken;
    }

    public class RedTriviaList<Lang> : RedNode {
        public TriviaList<Trivia<Lang>> GreenTriviaList { get; }

        protected RedTriviaList(TriviaList<Trivia<Lang>> greenTriviaList, RedToken<Lang>? parentToken = null) :
            base(parentToken) => GreenTriviaList = greenTriviaList;
    }

    public class RedTrivia<Lang> : RedNode {
        public Trivia<Lang> GreenTrivia { get; }

        protected RedTrivia(Trivia<Lang> greenTrivia, RedTriviaList<Lang>? parentTriviaList) : base(parentTriviaList) =>
            GreenTrivia = greenTrivia;
    }
}
