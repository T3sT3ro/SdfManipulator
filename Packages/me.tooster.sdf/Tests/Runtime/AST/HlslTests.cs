using System.Collections;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace me.tooster.sdf.Tests.Runtime.AST {
    public static class HlslTests {

        [UnityTest]
        public static IEnumerator affixTest() {
            var identifier = new Identifier {id = "foo"};
            var preInc = new Affix.Pre { id = identifier, prefixOperator = new PlusPlusToken() };
            var postInc = new Affix.Post { id = identifier, postfixOperator = new PlusPlusToken() };
            
            Assert.AreEqual("++foo", preInc.ToString());
            Assert.AreEqual("foo++", postInc.ToString());
            yield break;
        }
        
    }
}
