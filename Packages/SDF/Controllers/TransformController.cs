#nullable enable
using System;
using API;
using Assets.Nodes;
using PortData;
using Unity.VisualScripting.YamlDotNet.Serialization.NamingConventions;
using UnityEngine;

namespace Controllers {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Transform))]
    public class TransformController : Controller {
        protected Property<Matrix4x4> transformProperty;
        protected MatrixPropertyNode  primitiveTransform;

        private void Start() {
            transformProperty = sdfDomain.Graph.CreateProperty("transform", Matrix4x4.identity);
            primitiveTransform = new MatrixPropertyNode(transformProperty);
        }
        
        protected override void UpdateUniforms() {
            if (sdfDomain == null || sdfDomain.Material == null) 
                return;

            var spaceTransform = Matrix4x4.Rotate(Quaternion.Inverse(transform.localRotation));
            spaceTransform *=
                Matrix4x4.Translate(Vector3.Scale(-transform.localPosition, sdfDomain.transform.localScale));

            sdfDomain.Material.SetMatrix(((Representable)transformProperty).IdName,
                spaceTransform);
        }

        private void OnGUI() { throw new NotImplementedException(); }
    }
}
