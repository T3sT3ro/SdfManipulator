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

    // public class RedNode {
    //     public RedNode? Parent { get; }
    //     protected RedNode(RedNode? parent) { Parent = parent; }
    // }
    //
    // public class RedSyntax<Lang> : RedNode {
    //     public Syntax<Lang> Syntax { get; }
    //     protected RedSyntax(Syntax<Lang> syntax, RedNode? parent) : base(parent) { Syntax = syntax;  }
    // }
    //
    // public class RedToken<Lang> : RedNode {
    //     public Token<Lang> Token { get; }
    //     protected RedToken(Token<Lang> token, RedSyntax<Lang>? parent) : base(parent) { Token = token; }
    // }
    //
    // public class RedTriviaList<Lang> {
    //     public RedToken<Lang>?          PreviousToken { get; } 
    //     public RedToken<Lang>?          NextToken     { get; } 
    //     public TriviaList<Trivia<Lang>> TriviaList    { get; }
    //
    //     protected RedTriviaList(TriviaList<Trivia<Lang>> triviaList, RedToken<Lang>? previousToken = null, RedToken<Lang>? nextToken = null) {
    //         PreviousToken = previousToken;
    //         NextToken = nextToken;
    //         TriviaList = triviaList;
    //     }
    // }
    //
    // public class RedTrivia<T> : RedNode where T : Trivia<T> {
    //     public T Trivia { get; }
    //     protected RedTrivia(T trivia, RedNode? parent) : base(parent) { Trivia = trivia; }
    // }
}
