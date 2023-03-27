using System;
using API;
using static System.FormattableString;

namespace Builders {
    public abstract class PropertyBuilder : Builder<string, Property> {
        protected ShaderBuilder builder;
        public PropertyBuilder(ShaderBuilder builder) => this.builder = builder;

        /// <summary>
        /// Returns constructed string for a property
        /// </summary>
        /// <param name="property">property to generate shaderlab code</param>
        /// <returns></returns>
        public abstract string Build(Property property);
    }
}
