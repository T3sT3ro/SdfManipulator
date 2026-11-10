#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using me.tooster.sdf.Data;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEngine;
namespace me.tooster.sdf {
    /**
     * TODO: refactor to a editor + runtime type with conditional compilation.
     * Runtime would handle updating uniforms/keywords while editor handle updating the material and shader.
     */
    [Icon(SdfPaths.Icons.Sdf)]
    [ExecuteAlways]
    public abstract class Controller : MonoBehaviour, INotifyPropertyChanged, IModifier {
        public delegate void StructureChangedEventHandler(Controller sender);

        SdfScene? sdfScene;

        [SerializeField] [HideInInspector] string id = "";

        /// <summary>
        ///     A short identifier that is assigned once and stays the same when the object is renamed or moved. Generated
        ///     uniform and function names are built from it, see <see cref="SdfScene.RevalidateScene" />.
        /// </summary>
        public string Id => id;

        /// The names of the objects from the scene down to this controller, for comments and display names.
        public string HierarchyPath => SdfScene == null ? name : string.Join("/", SdfScene.GetControllerSceneAncestors(this).Reverse().Select(t => t.name));

#if UNITY_EDITOR
        /// Assigns a new random identifier, which is saved with the prefab.
        internal void AssignNewId() {
            id = Guid.NewGuid().ToString("N").Substring(0, 8);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        public SdfScene SdfScene => sdfScene == null ? sdfScene = GetComponentInParent<SdfScene>(true) : sdfScene;

        public SdfScene.PropertyData this[PropertyPath p] => SdfScene.sceneData.controllers[this].properties[p];

        protected virtual void OnEnable() { sdfScene = GetComponentInParent<SdfScene>(); }

        void OnDestroy() {
            MarkForRegeneration(sdfScene);
            StructureChanged?.Invoke(this);
        }

        void OnTransformParentChanged() {
            // both the scene that the controller left and the scene that it joined have to regenerate
            var previousScene = sdfScene;
            sdfScene = GetComponentInParent<SdfScene>(true);
            if (previousScene != sdfScene)
                MarkForRegeneration(previousScene);
            OnStructureChanged();
        }

        /// <summary>
        ///     Unity calls this after any change to the serialized data, also for changes that bypass the property setters,
        ///     e.g. an undo. Setting all uniforms on the next update keeps the rendering in sync with the data.
        /// </summary>
        protected virtual void OnValidate() {
            sdfScene = GetComponentInParent<SdfScene>();
            if (sdfScene != null)
                sdfScene.RequiresUpdate = true;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
#endif
        }

        public abstract IData Apply(IData input, Processor processor);
        public abstract Type  GetInputType();
        public abstract Type  GetOutputType();

        public event PropertyChangedEventHandler?  PropertyChanged;
        public event StructureChangedEventHandler? StructureChanged;

        /// <summary>
        ///     Marks the scene that contains this controller for regeneration, then notifies <see cref="StructureChanged" />
        ///     listeners. Controllers notify their scene directly. They used to forward the event through their parents and
        ///     copy subscriber lists on re-parenting, which could subscribe a controller to its own event and recurse until
        ///     the stack overflowed.
        /// </summary>
        public void OnStructureChanged() {
            MarkForRegeneration(SdfScene);
            StructureChanged?.Invoke(this);
        }

        static void MarkForRegeneration(SdfScene? scene) {
            if (scene != null)
                scene.RequiresRegeneration = true;
        }

        public void OnPropertyChanged([CallerMemberName] string? propertyName = null, Controller? sender = null) {
            PropertyChanged?.Invoke(sender == null ? this : sender, new PropertyChangedEventArgs(propertyName));
        }

        public bool SetField<T>(ref T field, T value, bool structural, [CallerMemberName] string? propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            if (structural)
                OnStructureChanged();
            else
                OnPropertyChanged(propertyName);
            return true;
        }

        public override string ToString()
            => SdfScene is { sceneData: { } sceneData } && sceneData.controllers.TryGetValue(this, out var c)
                ? $"{c.identifier} ({base.ToString()})"
                : base.ToString();
    }
}
