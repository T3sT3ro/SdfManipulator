using System.Collections;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using NUnit.Framework;

namespace me.tooster.sdf.Tests.Editor.AST {
    public class HlslTests {
        public static IEnumerator affixTest() {
            var identifier = new Identifier { id = "foo" };
            var preInc = new Affix.Pre { id = identifier, prefixOperator = new PlusPlusToken() };
            var postInc = new Affix.Post { id = identifier, postfixOperator = new PlusPlusToken() };

            Assert.AreEqual("++foo", preInc.ToString());
            Assert.AreEqual("foo++", postInc.ToString());
            yield break;
        }
        
        [Test]
        public static IEnumerator HlslSyntaxSmokeTest() {
            var varDeclaration = new VariableDeclarator
                { type = new Type.Predefined { typeToken = new MatrixToken() } };
            // Assert.AreEqual(varDeclaration, (varDeclaration.type as Type.Predefined)!.typeToken.Parent!.Parent);
            yield return true;
        }
    }
}
