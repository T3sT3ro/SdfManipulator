using System.Collections.Generic;
using API;
using Assets.Nodes;
using Assets.Nodes.MasterNodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Node = API.Node;

namespace Test.Editor.SDF {
    public static class ShaderGeneratorTest {
        [UnityTest]
        public static void TestShaderGenerator() {
            var vertNode = new VertexInNode();
            var v2fNode = new BasicVertToFragNode(
                vertNode.position,
                null,
                null,
                null
            );

            var other = new BasicVertToFragNode(v2fNode.position.In.Source, null, null, null);
            var fragmentNode = new UnlitFragOutNode(null);


            var targetNode = new BuiltInTargetNode("built_in_target");

            var graph = new Graph("test_graph", new HashSet<Node>()
            {
                vertNode,
                v2fNode,
                fragmentNode,
                targetNode
            });


            var shaderContent = graph.BuildShaderForTarget(targetNode);
            var shader = ShaderUtil.CreateShaderAsset(shaderContent);
            var graphName = ((Representable)graph).IdName;
            var targetName = ((Representable)targetNode).IdName;
            AssetDatabase.CreateAsset(shader, $"Test/GeneratedShaders/{graphName}-{targetName}.hlsl");
            var material = new Material(shader);
            AssetDatabase.CreateAsset(material, $"Test/GeneratedShaders/{graphName}-{targetName}.mat");
        }
    }
}
