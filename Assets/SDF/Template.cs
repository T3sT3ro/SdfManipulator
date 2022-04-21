using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SDF {
    /** Represents single node in the SDF tree of operators */
    [CreateAssetMenu(fileName = "SDF_Node", menuName = "SDF/Node", order = 0)]
    public class Template : ScriptableObject {

        [SerializeField] private ShaderInclude[] includes;
        
        
        [TextArea(1, Int32.MaxValue)]
        [SerializeField]
        private string template;

        // [Label("template string used to assemble final SDF expression")]
        // [ShowNativeProperty]

        public Tuple<Type, string>[] parameters;
        public Type output;


        private static Regex paramRegex = new Regex(@"_IN\s+(<type>\w)\s+(<name>\w)");
        private void OnValidate() {
            Console.WriteLine();
        }
        
    }

    public class TemplateInstance : MonoBehaviour {
        public Template template;
    }
}
