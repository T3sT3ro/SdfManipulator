using System;
using API;
using static System.FormattableString;

namespace Builders {
    public abstract class PropertyBuilder : Property.Visitor<FormattableString> {
        private ShaderBuilder builder;
        public PropertyBuilder(ShaderBuilder builder) => this.builder = builder;

        /// <summary>
        /// Returns constructed string for a property
        /// </summary>
        /// <param name="property">property to generate shaderlab code</param>
        /// <returns></returns>
        public virtual string Build(Property property) => Invariant(property.accept(this));

        // this method should never be called in practice, it's here only to satisfy the selective visitor pattern:
        // http://www.educery.com/papers/patterns/visitors/selective.visitor.html
        public FormattableString visit(Property property) =>
            throw new ShaderGenerationException($"Can't generate shaderlab property for '{property}'");
    }
}
