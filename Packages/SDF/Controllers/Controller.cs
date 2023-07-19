using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controllers {
    public abstract class Controller : MonoBehaviour {
        public Renderer sdfDomain;
        public Material sdfDomainSharedMaterial;

        private void OnEnable() { sdfDomainSharedMaterial = sdfDomain.sharedMaterial; }

        protected void Update() { UpdateUniforms(); }

        protected abstract void UpdateUniforms();
    }
}
