#nullable enable
using System.Text;

namespace me.tooster.sdf.AST.Syntax {
    public abstract record SyntaxOrToken<Lang> : Tree<Lang>.Node {
        // TODO: use it when declaring syntax to generate things like A a { get => _a; init => _a = value with { Parent = this }; }
        public Syntax<Lang>? Parent { get; init; } // TODO: fix visibility

        // without this, the debugger session and everything crashes on circular references:
        // https://github.com/dotnet/roslyn-analyzers/issues/5068
        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record#printmembers-formatting-in-derived-records
        protected override bool PrintMembers(StringBuilder builder) => false;

        // C#9 doesn't support sealed ToString, so any derived record must call base ToStiring();
        public override string ToString()    => base.ToString();
        public override int    GetHashCode() => base.GetHashCode();
    }
}
