using System;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Rendering;
using UnityEditor.Rendering.BuiltIn;
using UnityEngine;
using UnityEngine.Rendering;

namespace SDF {
    // [CreateAssetMenu(fileName = "SDF_Domain", menuName = "SDF/Domain", order = 0)]
    public class DomainTemplate : ScriptableObject {
        [SerializeField] private ShaderInclude engine;

        [Button]
        public void generateMaterial() { }
    }
}
