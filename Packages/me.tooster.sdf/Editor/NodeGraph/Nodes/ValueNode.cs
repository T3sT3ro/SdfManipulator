using System;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.PortData;
using UnityEngine;
using Identifier = me.tooster.sdf.AST.Hlsl.Syntax.Identifier;

namespace me.tooster.sdf.Editor.NodeGraph.Nodes {
    // base for things that provide simple output value
    // TODO: consider storing value in the node? ValueNode<TData, TValue> and TValue property?
    public abstract record ValueNode<TData> : Node where TData : Port.Data {
        public  IOutputPort<TData> Value     { get; }
        private Func<TData>        Evaluator { get; }

        protected ValueNode(string InternalName, string DisplayName, Func<TData> evaluator)
            : base(InternalName, DisplayName) {
            Evaluator = evaluator;
            Value = CreateOutput("value", Evaluator);
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
            : base("int_const", "Int Constant", () => new(new LiteralExpression { literal = (IntLiteral)value })) { }
    }


    public record FloatPropertyNode : PropertyNode<float, HlslScalar> {
        public FloatPropertyNode(Property<float> property)
            : base(property, () => new(new Identifier { id = property.InternalName })) { }
    }

    public record FloatConstantNode : ValueNode<HlslScalar> {
        public FloatConstantNode(float value)
            : base("float_const", "Float Constant",
                () => new(new LiteralExpression { literal = (FloatLiteral)value })) { }
    }


    public record VectorPropertyNode : PropertyNode<Vector4, HlslVector> {
        public VectorPropertyNode(Property<Vector4> property)
            : base(property, () => new(new Identifier { id = property.InternalName })) { }
    }

    public record VectorConstantNode : ValueNode<HlslVector> {
        public VectorConstantNode(Vector4 value)
            : base("vec4_const", "Vector4 Constant",
                () => new(new Call { id = "float4", argList = HlslUtil.VectorArgumentList(value) })) { }
    }


    public record MatrixPropertyNode : PropertyNode<Matrix4x4, HlslMatrix> {
        public MatrixPropertyNode(Property<Matrix4x4> property)
            : base(property, () => new(new Identifier { id = property.InternalName })) { }
    }

    public record MatrixConstantNode : ValueNode<HlslMatrix> {
        public MatrixConstantNode(Matrix4x4 m)
            : base("mat4_const", "Matrix 4x4 Constant", () => new(new Call
                { id = "float4x4", argList = HlslUtil.MatrixInitializerList(m) })
            ) { }
    }
}
