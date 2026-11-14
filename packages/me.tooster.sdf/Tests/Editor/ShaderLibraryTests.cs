using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>
    ///     Runs functions of the shader library on the GPU and compares their results. Needs a graphics device, so it's
    ///     skipped when Unity runs with <c>-nographics</c>.
    /// </summary>
    public class ShaderLibraryTests {
        const string Source = @"Shader ""Hidden/SdfShaderLibraryTest"" {
    SubShader {
        Pass {
            ZTest Always Cull Off ZWrite Off
            HLSLPROGRAM
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag
            #include ""UnityCG.cginc""
            #include ""Packages/me.tooster.sdf/ShaderLibrary/matrix.hlsl""
            #include ""Packages/me.tooster.sdf/ShaderLibrary/raymarching.hlsl""
            SdfResult sdfScene(in float3 p) { SdfResult r = (SdfResult)0; r.distance = length(p) - 1; return r; }
            #include ""Packages/me.tooster.sdf/ShaderLibrary/operators.hlsl""
            #include ""Packages/me.tooster.sdf/ShaderLibrary/primitives.hlsl""

            int _Case;

            float4 vert(float4 vertex : POSITION) : SV_POSITION { return UnityObjectToClipPos(vertex); }

            float4 frag() : SV_Target {
                float3 position; float4 rotation; float3 scale;
                switch (_Case) {
                    case 0: return float4(sdf::operators::repeat(float3(2.6, -2.6, 0.4), 1), 0);
                    case 1: return mul(m_scale(float3(2, 3, 4)), float4(1, 1, 1, 1));
                    case 2:
                        decompose(compose(float3(1, 2, 3), float4(0, 0, 0, 1), float3(1, 1, 1)), position, rotation, scale);
                        return float4(position, 0);
                    case 3: return float4(sdf::operators::elongate_fast(float3(3, -3, 0.5), 1), 0);
                    case 4: return mul(euler_to_rot(float3(0, 0, UNITY_PI / 2)), float4(1, 0, 0, 0));
                    default: return sdf::primitives3D::cylinder_rounded(float3(0, 1, 0), 0.5, 0.2, 1);
                }
            }
            ENDHLSL
        }
    }
}";

        static readonly int caseId = Shader.PropertyToID("_Case");

        Shader        shader;
        Material      material;
        RenderTexture target;
        Texture2D     readback;

        [SetUp]
        public void SetUp() {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
                Assert.Ignore("Running shaders needs a graphics device.");
            ShaderUtil.allowAsyncCompilation = false;
            shader = ShaderUtil.CreateShaderAsset(Source);
            material = new Material(shader);
            target = new RenderTexture(1, 1, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            readback = new Texture2D(1, 1, TextureFormat.RGBAFloat, false, true);
        }

        [TearDown]
        public void TearDown() {
            foreach (var o in new Object[] { readback, target, material, shader })
                if (o != null)
                    Object.DestroyImmediate(o);
        }

        Vector4 Run(int testCase) {
            material.SetInteger(caseId, testCase);
            Graphics.Blit(null, target, material);
            var previous = RenderTexture.active;
            RenderTexture.active = target;
            readback.ReadPixels(new Rect(0, 0, 1, 1), 0, 0);
            readback.Apply();
            RenderTexture.active = previous;
            return readback.GetPixel(0, 0);
        }

        // repeat used to mirror negative coordinates
        [TestCase(0, -0.4f, 0.4f, 0.4f, 0)]
        // m_scale put the z scale into the translation
        [TestCase(1, 2, 3, 4, 1)]
        // decompose read the translation from the bottom row instead of the last column
        [TestCase(2, 1, 2, 3, 0)]
        // elongate_fast returned a float
        [TestCase(3, 2, -2, 0, 0)]
        // euler_to_rot swapped its second and third rows, so a rotation about z turned x into z
        [TestCase(4, 0, 1, 0, 0)]
        // the top of a rounded cylinder with half height 1 is at height 1; it used to grow by the rounding
        [TestCase(5, 0, 0, 0, 0)]
        public void Helper_ReturnsTheExpectedValue(int testCase, float x, float y, float z, float w) {
            var result = Run(testCase);
            Assert.That(Vector4.Distance(result, new Vector4(x, y, z, w)), Is.LessThan(1e-4f), $"got {result}");
        }
    }
}
