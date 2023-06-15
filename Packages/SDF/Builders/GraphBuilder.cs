using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using API;

public class GraphBuilder : AbstractGraphBuilder {
    
    private          Graph                  graph;
    private readonly Evaluator              evaluator;
    private          Dictionary<Type, Type> builders;

    public GraphBuilder(Evaluator evaluator, Graph graph) : base(graph) {
        this.evaluator = evaluator;
        this.builders = FindBuilders();

        builders = FindBuilders();
    }

    // FIXME: assemblies may not be correct
    public static Dictionary<Type, Type> FindBuilders() {
        return typeof(NodeBuilder).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(NodeBuilder)))
            .ToDictionary(
                builderType => builderType.BaseType!.GetGenericArguments().First(),
                builderType => builderType
            );
    }

    public string Evaluate() {
        var variables = new List<Variable>();
        var includes = new HashSet<string>();
        foreach (var node in graph.TopologicalOrderIterator()) {
            includes.UnionWith(node.CollectIncludes());
            variables.AddRange(node.CollectVariables().Where(v => v.Exposed));
        }

        // this is where evaluator binding to nodes happens

        return this.evaluator(includes, variables, () => "", () => "", () => "");
    }

    public delegate string Evaluator(
        ISet<string>             includes,
        IEnumerable<Variable>    variables,
        VertInNode.Evaluator     vertIn,
        VertToFragNode.Evaluator v2f,
        FragOutNode.Evaluator    fragOut
    );
}
