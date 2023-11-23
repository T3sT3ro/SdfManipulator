using System.Collections;
using System.Collections.Generic;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.Nodes;
using me.tooster.sdf.Editor.NodeGraph.Nodes.MasterNodes;
using UnityEngine;
using UnityEngine.TestTools;
using Node = me.tooster.sdf.Editor.API.Node;

namespace me.tooster.sdf.Tests.Editor.SDF {
    public static class ShaderGeneratorTest {
        [UnityTest]
        public static IEnumerator TestShaderGenerator() {
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
            // var shader = ShaderUtil.CreateShaderAsset(shaderContent);
            var graphName = ((Representable)graph).IdName;
            var targetName = ((Representable)targetNode).IdName;
            Debug.Log(shaderContent);
            
            // AssetDatabase.CreateAsset(shader, $"Test/GeneratedShaders/{graphName}-{targetName}.hlsl");
            // var material = new Material(shader);
            // AssetDatabase.CreateAsset(material, $"Test/GeneratedShaders/{graphName}-{targetName}.mat");
            yield return null;
        }
    }
}
