using System;
using Logic.Nodes;

namespace Builders.BuiltInTarget.Nodes {
    public class PropertyNodeBuilder : PropertyNode.Visitor<FormattableString> {
        public FormattableString visit(PropertyNode node) => throw new NotImplementedException();
    }
}
