using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Controllers {
    [ExecuteInEditMode]
    public class TransformSdfController : SdfController {
        public string   uniformName = "_BoxPosition"; // todo: replace with identifier
        
        protected override void UpdateUniforms() {
            var spaceTransform = Matrix4x4.Rotate(Quaternion.Inverse(transform.localRotation));
            spaceTransform *= Matrix4x4.Translate(Vector3.Scale(-transform.localPosition , sdfDomain.transform.localScale));

            if (sdfDomainSharedMaterial != null) {
                sdfDomainSharedMaterial.SetMatrix(uniformName, spaceTransform);
            }
            else {
                Debug.LogWarning("No assigned renderer for controller " + this);
            }
        }
    }
}
