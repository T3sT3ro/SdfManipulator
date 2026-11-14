#nullable enable
using System.Linq;
using UnityEngine;

namespace me.tooster.sdf {
    /// <summary>
    ///     Makes a shader property of <see cref="target" /> read the uniform of the property with the same name on
    ///     <see cref="driver" />, so one value drives several controllers, e.g. the radius of several spheres. While the link
    ///     exists, the target's own value has no effect. Changing, adding or removing a link regenerates the shader.
    /// </summary>
    [AddComponentMenu("SDF/Property Link")]
    public class SdfPropertyLink : MonoBehaviour {
        [Tooltip("The controller whose property the driver drives")]
        public Controller? target;
        [Tooltip("The name of the shader property, e.g. Radius")]
        public string property = "";
        [Tooltip("The controller whose property of the same name drives the target")]
        public Controller? driver;

        // Unity calls Reset when the component is added: link the primitive on the same object, or another controller
        void Reset()
            => target = GetComponent<Primitives.SdfPrimitiveController>() as Controller
             ?? GetComponents<Controller>().FirstOrDefault(c => c is not SdfController and not TransformController);

        void OnValidate() {
            if (GetComponentInParent<SdfScene>() is not { } scene)
                return;
            scene.RequiresRegeneration = true;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
#endif
        }
    }
}
