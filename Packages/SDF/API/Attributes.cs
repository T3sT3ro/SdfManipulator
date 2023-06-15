using System;

namespace API {
    // attribute for node builder to get a list of strings representing the includes
    public class ShaderIncludeAttribute : Attribute {
        public string[] ShaderIncludes { get; }
        public ShaderIncludeAttribute(params string[] shaderIncludes) { ShaderIncludes = shaderIncludes; }
    }

    public class NodeEvaluatorAttribute : Attribute {
        public Type NodeType { get; }
        public NodeEvaluatorAttribute(Type nodeType) { NodeType = nodeType; }
    }
}
