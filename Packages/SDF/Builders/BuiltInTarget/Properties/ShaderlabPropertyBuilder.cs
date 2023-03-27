using System;
using API;
using UnityEngine;

// Jumbled as a single visitor because properties are sealed for extension
namespace Builders.BuiltInTarget.Properties {
    public class ShaderlabPropertyBuilder : PropertyBuilder {
        public ShaderlabPropertyBuilder(ShaderBuilder builder) : base(builder) { }

        // external properties don't generate shaderlab entries
        public override string Build(Property property) => property.External ? this.Build((dynamic)property) : null;

        private FormattableString Build(Property<int> property) =>
            $"{property.InternalName} (\"{property.DisplayName}\", Int) = {property.DefaultValue.ToString()}";

        private FormattableString Build(Property<float> property) =>
            $"{property.InternalName} (\"{property.DisplayName}\", Float) = {property.DefaultValue:F}";

        private FormattableString Build(Property<Vector4> property) =>
            $"{property.InternalName} (\"{property.DisplayName}\", Vector) = ({property.DefaultValue.x},{property.DefaultValue.y},{property.DefaultValue.z}, {property.DefaultValue.w})";
    }
}
