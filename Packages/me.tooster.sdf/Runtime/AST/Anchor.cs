#nullable enable
using System;

namespace me.tooster.sdf.AST {
    // TODO: add Node type to Anchor? or pattern match on Anchor<T>? when needed?
    // Instantiating would be possible only with https://stackoverflow.com/questions/8718199/passing-a-type-to-a-generic-constructor

    public interface IAnchor {
        public Anchor? Parent { get; }
        public object  Node   { get; }
    }

    /// <summary>
    /// A node with a parent, used for navigation. To pattern match on a node, use <see cref="Anchor{T}"/> like this:
    /// <code>someAnchor.Parent is Anchor&lt;SomeNode&gt; anchor</code>
    /// </summary>
    public abstract record Anchor(object Node, Anchor? Parent = null) : IAnchor {
        public object Node { get; protected init; } = Node;

        public static Anchor<T> New<T>(T value, Anchor? parent = null) => new(value, parent);

        public override string ToString() => "↑" + Node;
    }

    public interface IAnchor<out T> : IAnchor {
        Anchor? IAnchor.Parent => Parent;
        object IAnchor. Node   => Node;


        public new Anchor? Parent { get; }
        public new T       Node   { get; }
    }

    /// <summary>
    /// An <see cref="Anchor"/> holding a value of type <typeparamref name="T"/> 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed record Anchor<T> : Anchor, IAnchor<T> {
        public new T Node {
            get => (T)base.Node;
            private init => base.Node = value;
        }

        internal Anchor(T node, Anchor? parent = null) :
            base(node, parent) => Node = node;

        public static implicit operator T(Anchor<T> anchor) => anchor.Node;

        public override string ToString() => base.ToString();
    }

    public static class Extensions {
        public static Anchor<TBase> Cast<TBase>(this Anchor anchor) =>
            new Anchor<TBase>((TBase)anchor.Node, anchor.Parent);
        
        public static Anchor<T> CastTo<T>(this Anchor anchor, T _) => new Anchor<T>((T)anchor.Node, anchor.Parent);
    }
}
