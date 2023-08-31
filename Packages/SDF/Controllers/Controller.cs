#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using API;
using UnityEngine;

namespace Controllers {
    public abstract class Controller : MonoBehaviour {
        protected SdfSceneController sdfDomain = null!;

        protected void Start()    { sdfDomain = GetComponentInParent<SdfSceneController>(); }
        protected         void Update()   { UpdateUniforms(); }

        protected virtual void UpdateUniforms() { }

        // using reflection
        public IEnumerable<Node> CollectNodes() =>
            this.GetType().GetFields(BindingFlags.Instance)
                .Select(field => field.GetValue(this)).OfType<Node>();
    }
}
