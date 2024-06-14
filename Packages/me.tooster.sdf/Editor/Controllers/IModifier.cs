using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
namespace me.tooster.sdf.Editor.Controllers {
    // Base interface for all modifiers
    public interface IModifier {
        Data.IData Apply(Data.IData input, Processor processor);
        Type       GetInputType();
        Type       GetOutputType();
    }



    // Base class for strongly typed modifiers
    public interface IModifier<in TInput, out TOutput> : IModifier
        where TInput : Data.IData where TOutput : Data.IData {
        Data.IData IModifier.Apply(Data.IData input, Processor processor) => Apply((TInput)input, processor);
        public TOutput       Apply(TInput input, Processor processor);

        Type IModifier. GetInputType()  => GetInputType();
        Type IModifier. GetOutputType() => GetOutputType();
        public new Type GetInputType()  => typeof(TInput);
        public new Type GetOutputType() => typeof(TOutput);
    }



    // Evaluator that processes the modifier stack and handles requirements
    public interface Processor {
        public void HandleRequirement(Requirement requirement);

        public Data.IData ProcessStack(Data.IData input, IModifier[] modifiers) {
            return modifiers.Aggregate(input, (current, modifier) => modifier.Apply(current, this));
        }
    }



    public interface IRequirement {
        public IModifier Originator { get; }
    }



    public abstract class Requirement : IRequirement {
        public IModifier Originator { get; }
        protected Requirement(IModifier originator) => Originator = originator;
    }



    public class IncludeRequirement : Requirement {
        public string FileName { get; init; }

        public IncludeRequirement(IModifier originator, string fileName) : base(originator) => FileName = fileName;
    }



    public class FunctionDefinitionRequirement : Requirement {
        public FunctionDefinition functionDefinition { get; init; }

        public FunctionDefinitionRequirement(IModifier originator, FunctionDefinition functionDefinition) : base(originator)
            => this.functionDefinition = functionDefinition;
    }



    // Dynamic modifier stack, factory for typed stack
    [Serializable]
    public class ModifierStack : IModifier {
        readonly IModifier[] modifiers;

        ModifierStack(IEnumerable<IModifier> modifiers) {
            this.modifiers = modifiers.ToArray();
            ValidatePipeline();
        }

        public static ModifierStack Compose<TInput, TOutput>(params IModifier[] modifiers) {
            var stack = new ModifierStack(modifiers);
            if (stack.GetInputType() != typeof(TInput))
                throw new ArgumentException("Modifier stack expected input type " + typeof(TInput).Name);
            if (stack.GetOutputType() != typeof(TOutput))
                throw new ArgumentException("Modifier stack expected output type " + typeof(TOutput).Name);
            return stack;
        }

        public Data.IData Apply(Data.IData input, Processor processor) => processor.ProcessStack(input, modifiers);

        public Type GetInputType()  => modifiers[0].GetInputType();
        public Type GetOutputType() => modifiers[^1].GetOutputType();

        void ValidatePipeline() {
            if (modifiers.Length == 0)
                throw new ArgumentException("Modifier stack requires at least one modifier");

            for (var i = 0; i < modifiers.Length - 1; i++) {
                var producer = modifiers[i];
                var outputType = producer.GetOutputType();
                var consumer = modifiers[i + 1];
                var inputType = consumer.GetInputType();
                if (!inputType.IsAssignableFrom(outputType)) {
                    throw new InvalidOperationException(
                        $"Pipeline is invalid between modifiers at index {i} and {i + 1} in modifier {this}:"
                      + $"\n{outputType.Name} produced by {producer}"
                      + $"\n{inputType.Name} consumed by {consumer}"
                    );
                }
            }
        }
    }
}
