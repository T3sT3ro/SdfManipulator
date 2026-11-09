using System.Linq;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor;
using NUnit.Framework;
namespace me.tooster.sdf.Tests.Editor.AST {
    public class HlslTests {
        [Test]
        public static void AffixToString() {
            var identifier = new Identifier { id = "foo" };
            var preInc = new Affix.Pre { id = identifier, prefixOperator = new PlusPlusToken() };
            var postInc = new Affix.Post { id = identifier, postfixOperator = new PlusPlusToken() };

            Assert.AreEqual("++foo", preInc.ToString());
            Assert.AreEqual("foo++", postInc.ToString());
        }

        [TestCase(null, null, "#define FOO")]
        [TestCase(null, "1", "#define FOO 1")]
        [TestCase("x", "((x) * (x))", "#define FOO(x) ((x) * (x))")]
        public static void DefineToString(string argument, string tokenString, string expected) {
            var define = new Define
            {
                id = (Identifier)"FOO",
                argList = argument is null ? null : (ArgumentList<Identifier>)(Identifier)argument,
                tokenString = tokenString is null ? null : (TokenString)tokenString,
            };

            Assert.AreEqual(expected, HlslFormatter.Format(define)!.ToString().Trim());
        }

        [TestCase(new[] { "a" }, 0)]
        [TestCase(new[] { "a", "b" }, 1)]
        [TestCase(new[] { "a", "b", "c" }, 2)]
        public static void SeparatedList_CountsNodes(string[] identifiers, int expected) {
            ArgumentList<Identifier> args = identifiers.Select(i => (Identifier)i).ToArray();
            var list = args.arguments;

            Assert.AreEqual(identifiers.Length, list.Count);
            Assert.AreEqual(expected, list.SeparatorCount);
        }

        [TestCase(0, 2, "a,b")]
        [TestCase(1, 2, "b,c")]
        [TestCase(2, 1, "c")]
        [TestCase(1, 0, "")]
        public static void SeparatedList_SlicesNodesWithSeparators(int start, int length, string expected) {
            ArgumentList<Identifier> args = new Identifier[] { "a", "b", "c" };

            var slice = args.arguments.Slice<CommaToken>(start, length);

            Assert.AreEqual(expected, slice.ToString().Replace(" ", ""));
            Assert.AreEqual(length, slice.Count);
        }

        [Test]
        public static void HlslSyntaxSmokeTest() {
            var varDeclaration = new VariableDeclarator
                { type = new Type.Predefined { typeToken = new MatrixTypeToken() } };
            // Assert.AreEqual(varDeclaration, (varDeclaration.type as Type.Predefined)!.typeToken.Parent!.Parent);
        }
    }
}
