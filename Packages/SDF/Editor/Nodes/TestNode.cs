using System.Reflection;
using UnityEditor.ShaderGraph;

namespace Editor.Nodes {
    
    [Title("SDF", "Raymarch")]
    class TestNode : CodeFunctionNode{

        public TestNode() {
            name = "Raymarch";
        }
        protected override MethodInfo GetFunctionToConvert() => 
            GetType().GetMethod("Raymarch", BindingFlags.Static | BindingFlags.NonPublic);

        static string Raymarch(
            
        ) {
            
        }
    }
}
