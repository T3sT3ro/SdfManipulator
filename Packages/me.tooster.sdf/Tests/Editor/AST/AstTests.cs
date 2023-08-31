using System.Collections;
using System.Collections.Generic;
using AST.Hlsl.Syntax;
using AST.Syntax;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests.Editor.AST {

    interface FooLang {}
    interface BarLang {}
        
    record LangToken : Token<BarLang> {
        public override string Text => "BARLANG";
    }

    record BarInjected(Tree<BarLang> tree) : InjectedLanguage<FooLang, BarLang>(tree) {}
    
    abstract record Expr : Syntax<FooLang>;

    record Binary(Expr Left, Token<FooLang> Op, Expr Right) : Expr {
        public override IReadOnlyList<SyntaxOrToken<FooLang>> ChildNodesAndTokens =>
            new SyntaxOrToken<FooLang>[] { Left, Op, Right };
    }

    record Literal(Token<FooLang> Val) : Expr {
        public override IReadOnlyList<SyntaxOrToken<FooLang>> ChildNodesAndTokens => new[] { Val };
    }

    // with custom value
    record DynamicToken(string Value) : Token<FooLang> {
        public override string Text => Value;
    }

    abstract record OpToken : Token<FooLang>;

    record MulToken : OpToken {
        public override string Text => "*";
    }

    record SpaceToken : Token<FooLang> {
        public override string Text => " ";
    }

    record ZeroToken : Token<FooLang> {
        public override string Text => "0";
    }

    record NumToken(string val) : DynamicToken(val);


        // ========= TESTING =========

    public static class AbstractSyntaxTest {
        
        [UnityTest]
        public static IEnumerator FoobarSyntaxSmokeTest() {
            var oneToken = new NumToken("1");
            var oneLiteral = new Literal(oneToken);
            var zeroLiteral = new Literal(new ZeroToken());
            var op = new MulToken();
            var mulExpr = new Binary(oneLiteral, op, zeroLiteral);

            var mixedList = new SyntaxOrTokenList<FooLang>(oneLiteral, op, zeroLiteral, mulExpr);
            var x1 = mixedList[1];
            var x2 = mixedList[^1];
            var xs = mixedList[1..^2];
            var newMixedList = mixedList.Splice(0, 1, xs);

            var syntaxList = new SyntaxList<FooLang, Literal>(oneLiteral, zeroLiteral);
            var y1 = syntaxList[0];
            var y2 = syntaxList[^1];
            var ys = syntaxList[1..^2];
            var newSyntaxList = syntaxList.Splice(0, 1, ys);

            var separatedList = SeparatedList<FooLang, Literal>.With<SpaceToken>(oneLiteral, zeroLiteral);
            var z1 = separatedList[0];
            var z2 = separatedList[^1];

            var tree = new Tree<FooLang>(mulExpr);
            Assert.AreEqual("1*0", tree.ToString());
            var newMul = mulExpr with { Left = mulExpr };
            tree = new Tree<FooLang>(newMul);
            Assert.AreEqual("1*0*0", tree.ToString());
            yield return null;
        }

        [UnityTest]
        public static IEnumerator HlslSyntaxSmokeTest() {
            var varDeclaration = new VariableDeclarator { type = new Type.Predefined { typeToken = new MatrixToken() } };
            Assert.AreEqual(varDeclaration, (varDeclaration.type as Type.Predefined)!.typeToken.Parent!.Parent);
            yield return null;
        }
    }

}
