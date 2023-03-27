using System;
using API;
using UnityEngine;
using static System.FormattableString;

// Jumbled as a single visitor because properties are sealed for extension
namespace Builders.BuiltInTarget.Properties {
    public class HlslPropertyBuilder : PropertyBuilder {
        public HlslPropertyBuilder(ShaderBuilder builder) : base(builder) { }

        public override string Build(Property property) => Invariant(this.Build((dynamic)property));

        private FormattableString Build(Property<int>   property) => $"float {property.InternalName};";
        private FormattableString Build(Property<float> property) => $"float {property.InternalName};";
        private FormattableString Build(Property<Vector4>  property) => $"float4 {property.InternalName};";
    }
}
