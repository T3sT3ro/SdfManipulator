#nullable enable
using System.Text;

namespace me.tooster.sdf.AST.Syntax {
    public abstract partial record SyntaxOrToken<Lang> : IWriteable {
        // TODO: use it when declaring syntax to generate things like A a { get => _a; init => _a = value with { Parent = this }; }
        public          Syntax<Lang>? Parent { get; init; } // TODO: fix visibility
        public abstract StringBuilder WriteTo(StringBuilder sb);

        // without this, the debugger session and everything crashes on circular references:
        // https://github.com/dotnet/roslyn-analyzers/issues/5068
        protected virtual bool PrintMembers(StringBuilder builder) { return false; }

        // C#9 doesn't support sealed ToString, so any derived record must call base ToStiring();
        public override string ToString()    => WriteTo(new StringBuilder()).ToString();
        public override int    GetHashCode() => base.GetHashCode();
    }
}
