#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    /**
     * TODO: refactor to a editor + runtime type with conditional compilation.
     * Runtime would handle updating uniforms/keywords while editor handle updating the material and shader.
     */
    [Icon("Packages/me.tooster.sdf/Editor/Resources/Icons/sdf-icon-256.png")]
    [ExecuteAlways]
    public abstract class Controller : MonoBehaviour, INotifyPropertyChanged, IModifier {
        public delegate void StructureChangedEventHandler(Controller sender);

        SdfScene?       sdfScene;
        public SdfScene SdfScene => sdfScene == null ? sdfScene = GetComponentInParent<SdfScene>(true) : sdfScene;

        public SdfScene.PropertyData this[PropertyPath p] => SdfScene.sceneData.controllers[this].properties[p];

        protected virtual void OnValidate() { sdfScene = GetComponentInParent<SdfScene>(); }

        protected virtual void OnEnable() { sdfScene = GetComponentInParent<SdfScene>(); }

        void OnDestroy() { StructureChanged?.Invoke(this); }

        void OnTransformParentChanged() {
            sdfScene = GetComponentInParent<SdfScene>(true);
            OnStructureChanged(); // notify currently subscribed parent
            if (transform.GetComponentInParent<Controller>() is { } parentController)
                StructureChanged = parentController.StructureChanged; // clear all previous listeners as well
            StructureChanged?.Invoke(this);
        }

        public event PropertyChangedEventHandler?  PropertyChanged;
        public event StructureChangedEventHandler? StructureChanged;

        public void OnStructureChanged() => StructureChanged?.Invoke(this);

        public void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool SetField<T>(ref T field, T value, bool structural, [CallerMemberName] string? propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            if (structural) OnStructureChanged();
            else OnPropertyChanged(propertyName);
            return true;
        }

        public abstract IData Apply(IData input, Processor processor);
        public abstract Type  GetInputType();
        public abstract Type  GetOutputType();
    }
}
