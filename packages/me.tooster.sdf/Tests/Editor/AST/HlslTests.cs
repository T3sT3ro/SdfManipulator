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

        [Test]
        public static void HlslSyntaxSmokeTest() {
            var varDeclaration = new VariableDeclarator
                { type = new Type.Predefined { typeToken = new MatrixTypeToken() } };
            // Assert.AreEqual(varDeclaration, (varDeclaration.type as Type.Predefined)!.typeToken.Parent!.Parent);
        }
    }
}
