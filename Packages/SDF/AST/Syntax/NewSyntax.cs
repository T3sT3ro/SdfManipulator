#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AST.Syntax {
    public abstract record SyntaxOrToken<Lang> {
        public abstract void WriteTo(StringBuilder sb);

        public string BuildText() {
            var sb = new StringBuilder();
            WriteTo(sb);
            return sb.ToString();
        }
    }

    public abstract record Syntax<Lang> : SyntaxOrToken<Lang> {
        public          IReadOnlyList<Syntax<Lang>> ChildNodes => ChildNodesAndTokens.OfType<Syntax<Lang>>().ToList();
        public abstract IReadOnlyList<SyntaxOrToken<Lang>> ChildNodesAndTokens { get; }

        public override void WriteTo(StringBuilder sb) {
            foreach (var child in ChildNodesAndTokens)
                child.WriteTo(sb);
        }
    }

    public abstract record Token<Lang> : SyntaxOrToken<Lang> {
        public abstract string                      Text           { get; }
        public          IReadOnlyList<Trivia<Lang>> LeadingTrivia  { get; init; }
        public          IReadOnlyList<Trivia<Lang>> TrailingTrivia { get; init; }

        public override void WriteTo(StringBuilder sb) {
            foreach (var leading in LeadingTrivia) sb.Append(leading);
            sb.Append(Text);
            foreach (var trailing in LeadingTrivia) sb.Append(trailing);
        }
    }

    public record Trivia<Lang> {
        public         Token<Lang>   Parent    { get; init; }
        public virtual string?       Text      { get; init; }
        public virtual Syntax<Lang>? Structure { get; init; }
    }

    public record SyntaxOrTokenList<Lang>(IReadOnlyList<SyntaxOrToken<Lang>> List) : Syntax<Lang>,
        IReadOnlyList<SyntaxOrToken<Lang>> {
        public SyntaxOrTokenList(IEnumerable<SyntaxOrToken<Lang>> list) : this(list.ToList()) { }
        public SyntaxOrTokenList(params SyntaxOrToken<Lang>[]     list) : this(list.AsEnumerable()) { }
        public override IReadOnlyList<SyntaxOrToken<Lang>> ChildNodesAndTokens => List;
        public          IEnumerator<SyntaxOrToken<Lang>>   GetEnumerator()     => List.GetEnumerator();
        IEnumerator IEnumerable.                           GetEnumerator()     => GetEnumerator();
        public int                                         Count               => List.Count;

        public SyntaxOrToken<Lang> this[int index] => List[index];

        public SyntaxOrTokenList<Lang> Slice(int start, int length) =>
            new SyntaxOrTokenList<Lang>(List.Skip(start).Take(length));

        public SyntaxOrTokenList<Lang> Splice(int index, int deleteCount, IEnumerable<SyntaxOrToken<Lang>> elements)
            => new SyntaxOrTokenList<Lang>(List.Splice(index, deleteCount, elements));
    }

    // a variant that holds only syntax
    public record SyntaxList<Lang> : SyntaxOrTokenList<Lang> {
        public SyntaxList(IEnumerable<Syntax<Lang>>   list) : base(list) { }
        public SyntaxList(IReadOnlyList<Syntax<Lang>> List) : base(List) { }
        public SyntaxList(params Syntax<Lang>[]       list) : this(list.AsEnumerable()) { }

        public new Syntax<Lang> this[int      index] => (Syntax<Lang>)base[index];
        public new SyntaxList<Lang> Slice(int start, int length) => (SyntaxList<Lang>)base.Slice(start, length);

        public new SyntaxList<Lang> Splice(int index, int deleteCount, IEnumerable<SyntaxOrToken<Lang>> elements) =>
            (SyntaxList<Lang>)base.Splice(index, deleteCount, elements);
    }

    // a variant that holds SYNTAX TOKEN SYNTAX TOKEN ... SYNTAX
    public record SeparatedList<Lang> : SyntaxOrTokenList<Lang> {
        public SeparatedList(IEnumerable<SyntaxOrToken<Lang>>   list) : base(list) { }
        public SeparatedList(IReadOnlyList<SyntaxOrToken<Lang>> List) : base(List) { }
        public SeparatedList(params SyntaxOrToken<Lang>[]       list) : this(list.AsEnumerable()) { }

        // builds list from S1 S2 S3 -> S1 T1 S2 T2 S3 
        public static SeparatedList<Lang> Of<TTok>(IEnumerable<Syntax<Lang>> list) where TTok : Token<Lang>, new() =>
            new(list.SelectMany((x, i) => i == 0
                ? new SyntaxOrToken<Lang>[] { x }
                : new SyntaxOrToken<Lang>[] { new TTok(), x })
            );

        public static SeparatedList<Lang> Of<TTok>(params Syntax<Lang>[] list) where TTok : Token<Lang>, new() =>
            Of<TTok>(list.AsEnumerable());

        public new Syntax<Lang> this[int index] => (Syntax<Lang>)base[index << 1];

        /// returns a slice of the syntax nodes WITHOUT token nodes
        public new SyntaxList<Lang> Slice(int start, int length) => new SyntaxList<Lang>(List.OfType<Syntax<Lang>>().Skip(start).Take(length));
    }

//
    // ========= TESTING =========
//


    public interface Hlsl { }

    public interface Shaderlab { }

    public record ShaderlabToken : Token<Shaderlab> {
        public override string Text => "SHADERLAB";
    }

    public record Binary(Expr Left, Token<Hlsl> Op, Expr Right) : Syntax<Hlsl> {
        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens =>
            new SyntaxOrToken<Hlsl>[] { Left, Op, Right };
    }

    public abstract record Expr : Syntax<Hlsl>;

    public record Literal(Token<Hlsl> Val) : Expr {
        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new[] { Val };
    }

    // with custom value
    public record DynamicToken(string Value) : Token<Hlsl> {
        public override string Text => Value;
    }

    public abstract record OpToken : Token<Hlsl>;

    public record MulToken : OpToken {
        public override string Text => "*";
    }

    public record SpaceToken : Token<Hlsl> {
        public override string Text => " ";
    }

    public record ZeroToken : Token<Hlsl> {
        public override string Text => "0";
    }

    public record NumToken(string val) : DynamicToken(val);

    public static class Test {
        public static void test() {
            var oneToken = new NumToken("1");
            var oneLiteral = new Literal(oneToken);
            var zeroLiteral = new Literal(new ZeroToken());
            var op = new MulToken();
            var bin = new Binary(oneLiteral, op, zeroLiteral);

            var mixedList = new SyntaxOrTokenList<Hlsl>(oneLiteral, op, zeroLiteral);
            var x1 = mixedList[1];
            var x2 = mixedList[^1];
            var xs = mixedList[1..^2];
            var newMixedList = mixedList.Splice(0, 1, xs);

            var syntaxList = new SyntaxList<Hlsl>(oneLiteral, zeroLiteral);
            var y1 = syntaxList[0];
            var y2 = syntaxList[^1];
            var ys = syntaxList[1..^2];
            var newSyntaxList = syntaxList.Splice(0, 1, ys);

            var separatedList = SeparatedList<Hlsl>.Of<SpaceToken>(oneLiteral, zeroLiteral);
            var z1 = separatedList[0];
            var z2 = separatedList[^1];
            var zs = separatedList[1..^2];
            var newSeparatedList = separatedList.Splice(0, 1, zs); // welp, whatchu gonna do, amirite?
        }
    }
}
