#nullable enable
namespace me.tooster.sdf.AST {
    // TODO: try to use a red-green tree like in Roslyn auto-generated from annotations
    // Instantiating would be possible only with https://stackoverflow.com/questions/8718199/passing-a-type-to-a-generic-constructor

    /// <summary>
    /// A node with a parent, used for navigation
    /// </summary>
    public abstract record Anchor {
        public Anchor? Parent { get; }
        protected Anchor(Anchor? parent = null) { Parent = parent; }
    }

    /// <summary>
    /// An <see cref="Anchor"/> holding a value of type <typeparamref name="T"/> 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed record Anchor<T> : Anchor {
        public T Node { get; }
        public Anchor(T node, Anchor? parent = null) : base(parent) { Node = node; }
    }
}
