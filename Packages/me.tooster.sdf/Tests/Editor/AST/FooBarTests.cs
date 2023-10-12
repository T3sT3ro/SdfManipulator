using System.Collections;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.Tests.Runtime.AST.BarLang;
using me.tooster.sdf.Tests.Runtime.AST.FooLang;
using NUnit.Framework;
using UnityEngine.TestTools;

// ReSharper disable NotAccessedPositionalProperty.Global
namespace me.tooster.sdf.Tests.Runtime.AST.BarLang {
    public interface BarLang { }
    // with custom value
}

namespace me.tooster.sdf.Tests.Runtime.AST.FooLang {
    public interface FooLang { }

    // syntax parts
    [Syntax] partial record BarInjected(Tree<BarLang.BarLang> tree) : InjectedLanguage<FooLang, BarLang.BarLang>(tree) {
        public BarInjected() : this(new Tree<BarLang.BarLang>()) { }
    }

    [Syntax] abstract partial record Expr : Syntax<FooLang>;

    [Syntax] public partial record Binary : Expr {
        private readonly Expr           _left;
        private readonly Token<FooLang> _op;
        private readonly Expr           _right;
    }

    [Syntax] public partial record Literal : Expr {
        private readonly Token<FooLang> _val;
    }
    
    // @formatter off
    record LangToken : Token<BarLang.BarLang>          { public override string Text => "BARLANG"; }
    abstract record OpToken : Token<FooLang>;
    record MulToken   : OpToken                        { public override string Text => "*"; }
    record SpaceToken : Token<FooLang>                 { public override string Text => " "; }
    record ZeroToken  : Token<FooLang>                 { public override string Text => "0"; }
    abstract record DynamicToken(string Value) : Token<FooLang> { public override string Text => Value; }
    record NumToken(string val) : DynamicToken(val);
    // @formatter on
}

// ReSharper enable NotAccessedPositionalProperty.Global

namespace me.tooster.sdf.Tests.Runtime.AST {
    // ========= TESTING =========
    // ReSharper disable UnusedVariable

    public static class AbstractSyntaxTest {
        [UnityTest]
        public static IEnumerator FoobarSyntaxSmokeTest() {
            var oneToken = new NumToken("1");
            var oneLiteral = new Literal {val = oneToken};
            var zeroLiteral = new Literal { val = new ZeroToken()};
            var op = new MulToken();
            var mulExpr = new Binary { left = oneLiteral, op = op, right = zeroLiteral};

            var mixedList = new SyntaxOrTokenList<FooLang.FooLang>(oneLiteral, op, zeroLiteral, mulExpr);
            var x1 = mixedList[1];
            var x2 = mixedList[^1];
            var xs = mixedList[1..^2];
            var newMixedList = mixedList.Splice(0, 1, xs);

            var syntaxList = new SyntaxList<FooLang.FooLang, Literal>(oneLiteral, zeroLiteral);
            var y1 = syntaxList[0];
            var y2 = syntaxList[^1];
            var ys = syntaxList[1..^2];
            var newSyntaxList = syntaxList.Splice(0, 1, ys);

            var separatedList = SeparatedList<FooLang.FooLang, Literal>.With<SpaceToken>(oneLiteral, zeroLiteral);
            var z1 = separatedList[0];
            var z2 = separatedList[^1];

            var tree = new Tree<FooLang.FooLang>(mulExpr);
            Assert.AreEqual("1*0", tree.ToString());
            var newMul = mulExpr with { left = mulExpr };
            tree = new Tree<FooLang.FooLang>(newMul);
            Assert.AreEqual("1*0*0", tree.ToString());
            yield return null;
        }
        // ReSharper enable UnusedVariable
    }
}