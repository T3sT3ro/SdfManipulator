using System;
using API;
using BuiltInTarget.Variables;
using Nodes.MasterNodes;
using UnityEngine;

namespace BuiltInTarget.Nodes.MasterNodes {
    public class BasicVertToFragNodeBuilder : NodeBuilder<BasicVertToFragNode> {
        public BasicVertToFragNodeBuilder(BasicVertToFragNode node) : base(node) { }

        public string GetV2fStructDefinition => $@"
struct v2f {{
    float4 {nameof(node.position)}  : SV_POSITION;
    float4 {nameof(node.normal)}    : TEXCOORD1;
    float4 {nameof(node.texCoord)}  : TEXCOORD2;
    float4 {nameof(node.color)}     : TEXCOORD3;
}};
";

        public VertToFragNode.FragmentEvaluator GetEvaluator(
            Variable<Vector4>.Evaluator positionEvaluator,
            Variable<Vector4>.Evaluator normalEvaluator,
            Variable<Vector4>.Evaluator texCoordEvaluator,
            Variable<Vector4>.Evaluator colorEvaluator
        ) {
            return () => $@"
v2f v;
{GetSelectorFor(node.position)("v")} = {positionEvaluator};
{GetSelectorFor(node.normal)("v")} = {normalEvaluator};
{GetSelectorFor(node.texCoord)("v")} = {texCoordEvaluator};
{GetSelectorFor(node.color)("v")} = {colorEvaluator};
";
        }

        public override NodeBuilder.Selector GetSelectorFor(InputPort port) {
            if (port == node.position) return value => $"{value}.{nameof(BasicVertToFragNode.position)}";
            if (port == node.normal) return value => $"{value}.{nameof(BasicVertToFragNode.normal)}";
            if (port == node.texCoord) return value => $"{value}.{nameof(BasicVertToFragNode.texCoord)}";
            if (port == node.color) return value => $"{value}.{nameof(BasicVertToFragNode.color)}";

            throw new ArgumentOutOfRangeException(nameof(port), "This port doesn't belong to this node");
        }
    }
}
