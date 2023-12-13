#nullable enable
namespace me.tooster.sdf.AST {
    // TODO: add Node type to Anchor? or pattern match on Anchor<T>? when needed?
    // Instantiating would be possible only with https://stackoverflow.com/questions/8718199/passing-a-type-to-a-generic-constructor

    /// <summary>
    /// A node with a parent, used for navigation. To pattern match on a node, use <see cref="Anchor{T}"/> like this:
    /// <code>someAnchor.Parent is Anchor&lt;SomeNode&gt; anchor</code>
    /// </summary>
    public abstract record Anchor {
        public Anchor? Parent { get; }
        protected Anchor(Anchor? parent = null) => Parent = parent;

        public static Anchor<T> New<T>(T value, Anchor? other = null) => new(value, other);
    }

    /// <summary>
    /// An <see cref="Anchor"/> holding a value of type <typeparamref name="T"/> 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed record Anchor<T> : Anchor{
        public T Node { get; }
        internal Anchor(T node, Anchor? parent = null) : base(parent) => Node = node;
        
        public static implicit operator T(Anchor<T> anchor) => anchor.Node;
    }
}
