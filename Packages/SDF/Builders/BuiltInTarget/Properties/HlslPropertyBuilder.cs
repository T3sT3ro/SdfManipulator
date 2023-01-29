using System;
using Logic.Properties;

// Jumbled as a single visitor because properties are sealed for extension
namespace Builders.BuiltInTarget.Properties {
    public class HlslPropertyBuilder : PropertyBuilder,
                                       IntProperty.Visitor<FormattableString>,
                                       FloatProperty.Visitor<FormattableString>,
                                       Vec4Property.Visitor<FormattableString> {
        public HlslPropertyBuilder(ShaderBuilder builder) : base(builder) { }

        public FormattableString visit(IntProperty   property) => $"float {property.InternalName};";
        public FormattableString visit(FloatProperty property) => $"float {property.InternalName};";
        public FormattableString visit(Vec4Property  property) => $"float4 {property.InternalName};";
    }
}
