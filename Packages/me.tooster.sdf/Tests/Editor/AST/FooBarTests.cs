using System.Collections;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.BarLang;
using me.tooster.sdf.AST.FooLang;
using NUnit.Framework;

// ReSharper disable NotAccessedPositionalProperty.Global
namespace me.tooster.sdf.AST.BarLang {
    public interface barlang { }
    // with custom value
}

namespace me.tooster.sdf.AST.FooLang {
    // syntax parts
    public record BarInjected(Tree<barlang> tree) : InjectedLanguage<foolang, barlang>(tree) {
        public BarInjected() : this(new Tree<barlang>()) { }
    }

    [SyntaxNode] public abstract partial record Expr;

    [SyntaxNode] public partial record Binary : Expr {
        public Expr           left  { get; init; }
        public Token<foolang> op    { get; init; }
        public Expr           right { get; init; }
    }

    [SyntaxNode] public partial record Literal : Expr {
        public Token<foolang> val { get; init; }
    }
    
    // @formatter off
    record LangToken : Token<barlang>          { public override string FullText => "BARLANG"; }
    abstract record OpToken : Token<foolang>;
    record MulToken   : OpToken                        { public override string FullText => "*"; }
    record SpaceToken : Token<foolang>                 { public override string FullText => " "; }
    record ZeroToken  : Token<foolang>                 { public override string FullText => "0"; }
    abstract record DynamicToken(string Value) : Token<foolang> { public override string FullText => Value; }
    record NumToken(string val) : DynamicToken(val);
    // @formatter on
}

// ReSharper enable NotAccessedPositionalProperty.Global

namespace me.tooster.sdf.Tests.Editor.AST {
    // ========= TESTING =========
    
    public class FoobarSyntaxTests {
        [Test]
        public void FoobarSyntaxSmokeTest() {
            var oneToken = new NumToken("1");
            var oneLiteral = new Literal { val = oneToken };
            var zeroLiteral = new Literal { val = new ZeroToken() };
            var op = new MulToken();
            var mulExpr = new Binary { left = oneLiteral, op = op, right = zeroLiteral };

            var mixedList = new SyntaxOrTokenList<foolang>(oneLiteral, op, zeroLiteral, mulExpr);
            var x1 = mixedList[1];
            var x2 = mixedList[^1];
            var xs = mixedList[1..^2];
            var newMixedList = mixedList.Splice(0, 1, xs);

            var syntaxList = new SyntaxList<foolang, Literal>(oneLiteral, zeroLiteral);
            var y1 = syntaxList[0];
            var y2 = syntaxList[^1];
            var ys = syntaxList[1..^2];
            var newSyntaxList = syntaxList.Splice(0, 1, ys);

            var separatedList = SeparatedList<foolang, Literal>.With<SpaceToken>(oneLiteral, zeroLiteral);
            var z1 = separatedList[0];
            var z2 = separatedList[^1];

            var tree = new Tree<foolang>(mulExpr);
            Assert.AreEqual("1*0", tree.Text);
            var newMul = mulExpr with { left = mulExpr };
            tree = new Tree<foolang>(newMul);
            Assert.AreEqual("1*0*0", tree.Text);
        }
    }
}
