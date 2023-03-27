using System;
using System.Collections.Generic;
using Builders;
using Builders.BuiltInTarget;
using Builders.BuiltInTarget.Nodes;
using Logic.Nodes;
using Logic.Nodes.SdfNodes;
using UnityEngine.TestTools;

namespace Tests.SDF {
    public class ShaderGeneratorTest {
        [UnityTest]
        public static void TestShaderGenerator() {
            var masterNode = new UnlitRaymarcherMasterNode();
            // var colorNode = new PropertyNode<Vec4Property>("vec4_property", "vec4 proeprty node", true, new Vec4Property());
            // TODO: internal/display names extract elsewhere
            var sdfNode = new SdfSphereNode();
            
            var generatorMapping = new Dictionary<Type, Type>
            {
                [typeof(PropertyNode)] = typeof(PropertyNodeBuilder),
                [typeof(UnlitRaymarcherMasterNode)] = typeof(UnlitRaymarcherMasterNodeBuilder),
            };
            
            // TODO: lepiej zrobić mapę node <-> generator, czy może po prostu SdfNodeBuilder<SdfNode> czy coś?
            var shaderBuilder = new ShaderBuilder(generatorMapping); 
            shaderBuilder.Build(masterNode);
        }
    }
}
