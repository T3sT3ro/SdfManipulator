using UnityEditor;
using UnityEngine;

namespace Controllers {


    public abstract class Controller : MonoBehaviour {
        public Renderer sdfDomain;
        public Material sdfDomainSharedMaterial;

        private void OnEnable() { sdfDomainSharedMaterial = sdfDomain.sharedMaterial; }

        protected void Update() { UpdateUniforms(); }

        protected abstract void UpdateUniforms();

        [MenuItem("GameObject/SDF/Primitive/Box")]
        public void CreateSdfNode() { }
    }
}
