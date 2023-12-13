#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using me.tooster.sdf.Editor.API;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers {
    public abstract class Controller : MonoBehaviour {
        protected                                                         SdfSceneController sdfDomain = null!;

        protected virtual void Start()    { sdfDomain = GetComponentInParent<SdfSceneController>(); }
        protected virtual void Update()   { UpdateUniforms(); }

        protected virtual void UpdateUniforms() { }

        // using reflection
        public IEnumerable<Node> CollectNodes() =>
            this.GetType().GetFields(BindingFlags.Instance)
                .Select(field => field.GetValue(this)).OfType<Node>();
    }
}
