using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.Data;
namespace me.tooster.sdf {
    // Base interface for all modifiers
    public interface IModifier {
        IData Apply(IData input, Processor processor);
        Type  GetInputType();
        Type  GetOutputType();
    }



    // Base class for strongly typed modifiers
    public interface IModifier<in TInput, out TOutput> : IModifier
        where TInput : IData where TOutput : IData {
        IData IModifier.Apply(IData input, Processor processor) => Apply((TInput)input, processor);

        Type IModifier.    GetInputType()  => GetInputType();
        Type IModifier.    GetOutputType() => GetOutputType();
        public     TOutput Apply(TInput input, Processor processor);
        public new Type    GetInputType()  => typeof(TInput);
        public new Type    GetOutputType() => typeof(TOutput);
    }



    // Evaluator that processes the modifier stack and handles requirements
    public abstract class Processor {
        readonly        Stack<IModifier>   modifierEvaluationStack = new();
        readonly        HashSet<IModifier> pendingModifiers        = new();
        public abstract void               HandleRequirement(Requirement requirement);

        public IData Process(IData input, params IModifier[] modifiers) {
            return modifiers.Aggregate(
                input,
                (current, modifier) => {
                    if (pendingModifiers.Contains(modifier)) {
                        throw new InvalidOperationException(
                            "Cyclic dependency detected when tryin to process modifiers. Evaluation stack:\n"
                          + string.Join("\n", modifierEvaluationStack)
                        );
                    }

                    modifierEvaluationStack.Push(modifier);
                    pendingModifiers.Add(modifier);
                    var returnValue = modifier.Apply(current, this);
                    modifierEvaluationStack.Pop();
                    pendingModifiers.Remove(modifier);
                    return returnValue;
                }
            );
        }
    }



    public interface IRequirement {
        public IModifier Originator { get; }
    }



    public abstract class Requirement : IRequirement {
        protected Requirement(IModifier originator) => Originator = originator;
        public IModifier Originator { get; }
    }



    public class IncludeRequirement : Requirement {
        public IncludeRequirement(IModifier originator, string fileName) : base(originator) => FileName = fileName;
        public string FileName { get; init; }
    }



    public class FunctionDefinitionRequirement : Requirement {
        public FunctionDefinitionRequirement(IModifier originator, FunctionDefinition functionDefinition) : base(originator)
            => this.functionDefinition = functionDefinition;
        public FunctionDefinition functionDefinition { get; init; }
    }



    // Dynamic modifier stack, factory for typed stack
    [Serializable]
    public class ModifierStack : IModifier {
        readonly IModifier[] modifiers;

        ModifierStack(IEnumerable<IModifier> modifiers) {
            this.modifiers = modifiers.ToArray();
            ValidatePipeline();
        }

        public IData Apply(IData input, Processor processor) => processor.Process(input, modifiers);

        public Type GetInputType()  => modifiers[0].GetInputType();
        public Type GetOutputType() => modifiers[^1].GetOutputType();

        public static ModifierStack Compose<TInput, TOutput>(params IModifier[] modifiers) {
            var stack = new ModifierStack(modifiers);
            if (stack.GetInputType() != typeof(TInput))
                throw new ArgumentException("Modifier stack expected input type " + typeof(TInput).Name);
            if (stack.GetOutputType() != typeof(TOutput))
                throw new ArgumentException("Modifier stack expected output type " + typeof(TOutput).Name);
            return stack;
        }

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
