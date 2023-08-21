using System;
using System.Linq;
using API;
using AST.Hlsl.Syntax;
using AST.Hlsl.Syntax.Expressions;
using AST.Hlsl.Syntax.Expressions.Operators;
using PortData;
using UnityEngine;

namespace Nodes {
    // base for things that provide simple output value
    public abstract record ValueNode<TData> : Node where TData : Port.Data {
        public  OutputPort<TData> Value     { get; }
        private Func<TData>       Evaluator { get; init; }

        protected ValueNode(string InternalName, string DisplayName, Func<TData> evaluator)
            : base(InternalName, DisplayName) {
            this.Evaluator = evaluator;
            this.Value = this.CreateOutput("value", evaluator);
        }
    }

    // properties inherit names from their nodes
    public record PropertyNode<TProperty, TData> : ValueNode<TData> where TData : Port.Data {
        public Property<TProperty> Property { get; init; }

        /// Creates a node from some existing property. Several nodes can reference the same property
        public PropertyNode(Property<TProperty> property, Func<TData> evaluator)
            : base(property.InternalName, property.DisplayName, evaluator) {
            Property = property;
        }
    }

    // TODO: refactor nodes to handle both constant and property at the same time, with option to switch between the two? 
    public record IntPropertyNode : PropertyNode<int, HlslScalar> {
        public IntPropertyNode(Property<int> property)
            : base(property, () => new(new Identifier { id = property.InternalName })) { }
    }

    // TODO: refactor to bake constant syntax while building shader but while editing use uniforms
    public record IntConstantNode : ValueNode<HlslScalar> {
        public IntConstantNode(int value)
            : base("int_const", "Int Constant", () => new(new LiteralExpression<IntLiteral> { literal = value })) { }
    }


    public record FloatPropertyNode : PropertyNode<float, HlslScalar> {
        public FloatPropertyNode(Property<float> property)
            : base(property, () => new(new Identifier { id = property.InternalName })) { }
    }

    public record FloatConstantNode : ValueNode<HlslScalar> {
        public FloatConstantNode(float value)
            : base("float_const", "Float Constant",
                () => new(new LiteralExpression<FloatLiteral> { literal = value })) { }
    }


    public record VectorPropertyNode : PropertyNode<Vector4, HlslVector> {
        public VectorPropertyNode(Property<Vector4> property)
            : base(property, () => new(new Identifier { id = property.InternalName })) { }
    }

    public record VectorConstantNode : ValueNode<HlslVector> {
        public VectorConstantNode(Vector4 value)
            : base("vec4_const", "Vector4 Constant",
                () => new(new Call { id = "float4", arguments = HlslUtil.VectorArgumentList(value) })) { }
    }


    public record MatrixPropertyNode : PropertyNode<Matrix4x4, HlslMatrix> {
        public MatrixPropertyNode(Property<Matrix4x4> property)
            : base(property, () => new(new Identifier { id = property.InternalName })) { }
    }

    public record MatrixConstantNode : ValueNode<HlslMatrix> {
        public MatrixConstantNode(Matrix4x4 m)
            : base("mat4_const", "Matrix 4x4 Constant",
                () => new(new Call
                {
                    id = "float4x4", arguments = HlslUtil.MatrixInitializerList(m).ToArgumentList()
                })) { }
    }
}
