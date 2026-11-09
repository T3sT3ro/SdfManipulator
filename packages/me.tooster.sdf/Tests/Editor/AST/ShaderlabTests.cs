using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.Commands;
using NUnit.Framework;

namespace me.tooster.sdf.Tests.Editor.AST {
    public class ShaderlabTests {
        [Test]
        public static void BlendRgbaFactors_PrintRgbBeforeAlpha() {
            // Blend SrcRGB DstRGB, SrcA DstA
            var arguments = new Blend.RgbaFactorArguments
            {
                srcRgbFactor = new OneKeyword(),
                dstRgbFactor = new ZeroKeyword(),
                srcAlphaFactor = new SrcAlphaKeyword(),
                dstAlphaFactor = new DstAlphaKeyword(),
            };

            Assert.AreEqual("OneZero,SrcAlphaDstAlpha", arguments.ToString().Replace(" ", ""));
        }
    }
}
