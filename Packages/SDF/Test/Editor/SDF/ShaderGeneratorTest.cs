using API;
using BuiltInTarget;
using Nodes;
using Nodes.MasterNodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Test.Editor.SDF {
    public static class ShaderGeneratorTest {
        [UnityTest]
        public static void TestShaderGenerator() {
            
            var graph = new Graph(
                new VertexInNode(),
                new BasicVertToFragNode(),
                new UnlitFragOutNode()
            );

            var vertNode = new VertexInNode();
            var v2fNode = new BasicVertToFragNode();
            var fragmentNode = new UnlitFragOutNode();
            var colorNode = new VariableNode<Vector4>("vec4_property",
                "vec4 property node",
                true,
                new Variable<Vector4>("color", "color", new Vector4(1,0,1,0)));
            
            colorNode.Value.ConnectTo(fragmentNode.color);
            
            var graphBuilder = new GraphBuilder(UnlitShaderBuilder.evaluator, graph);

            var shaderContent = graphBuilder.Evaluate();
            var shader = ShaderUtil.CreateShaderAsset(shaderContent);
            AssetDatabase.CreateAsset(shader, "Test/GeneratedShaders/SimpleShader.hlsl");
            var material = new Material(shader);
            AssetDatabase.CreateAsset(shader, "Test/GeneratedShaders/SimpleShader.mat");
        }
    }
}
