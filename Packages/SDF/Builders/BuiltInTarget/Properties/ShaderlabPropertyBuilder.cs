using System;
using API;
using JetBrains.Annotations;
using Logic.Properties;

// Jumbled as a single visitor because properties are sealed for extension
namespace Builders.BuiltInTarget.Properties {
    public class ShaderlabPropertyBuilder : PropertyBuilder,
                                            IntProperty.Visitor<FormattableString>,
                                            FloatProperty.Visitor<FormattableString>,
                                            Vec4Property.Visitor<FormattableString> {
        public ShaderlabPropertyBuilder(ShaderBuilder builder) : base(builder) { }

        // external properties don't generate shaderlab entries
        [CanBeNull] public override string Build(Property property) => property.External ? base.Build(property) : null;

        public FormattableString visit(IntProperty property) =>
            $"{property.InternalName} (\"{property.DisplayName}\", Int) = {property.DefaultValue.ToString()}";

        public FormattableString visit(FloatProperty property) =>
            $"{property.InternalName} (\"{property.DisplayName}\", Float) = {property.DefaultValue:F}";

        public FormattableString visit(Vec4Property property) =>
            $"{property.InternalName} (\"{property.DisplayName}\", Vector) = ({property.DefaultValue.x},{property.DefaultValue.y},{property.DefaultValue.z}, {property.DefaultValue.w})";
    }
}
