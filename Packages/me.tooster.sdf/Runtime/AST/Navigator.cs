namespace me.tooster.sdf.AST {
    public class Navigator {
        // TODO: try to use a red-green tree like in Roslyn auto-generated from annotations
        // Instantiating would be possible only with https://stackoverflow.com/questions/8718199/passing-a-type-to-a-generic-constructor
        public abstract class Navigable {
            public Navigable Parent { get; }
            protected Navigable(Navigable parent) { Parent = parent; }
        }

        public sealed class Navigable<T> : Navigable {
            public T Value { get; }
            public Navigable(T value, Navigable parent) : base(parent) { Value = value; }
        }
    }
}
