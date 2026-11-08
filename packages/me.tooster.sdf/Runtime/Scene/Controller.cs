#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        protected virtual void OnValidate() { sdfScene = GetComponentInParent<SdfScene>(); }

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
