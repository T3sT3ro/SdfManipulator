using API;
using Nodes;
using Nodes.MasterNodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Test.Editor.SDF {
    public static class ShaderGeneratorTest {
        [UnityTest]
        public static void TestShaderGenerator() {

            var vertNode = new VertexInNode();
            var v2fNode = new BasicVertToFragNode();
            var fragmentNode = new UnlitFragOutNode();
            var colorNode = new ValueNode<,>("vec4_property",
                "vec4 property node",
                true,
                new Property<Vector4>("color", "color", new Vector4(1,0,1,0)));
            
            colorNode.Value.ConnectTo(fragmentNode.color);
            
            var targetNode = new BuiltInTargetNode("built_in_target");
            
            var graph = new Graph(
                new VertexInNode(),
                new BasicVertToFragNode(),
                new UnlitFragOutNode()
            );

            
            var shaderContent = graph.BuildShaderForTarget(targetNode);
            var shader = ShaderUtil.CreateShaderAsset(shaderContent);
            AssetDatabase.CreateAsset(shader, $"Test/GeneratedShaders/{}.hlsl");
            var material = new Material(shader);
            AssetDatabase.CreateAsset(shader, $"Test/GeneratedShaders/SimpleShader.mat");
        }
        
        
    }
}
