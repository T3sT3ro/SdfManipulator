#nullable enable
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    /**
     * TODO: refactor to a editor + runtime type with conditional compilation.
     * Runtime would handle updating uniforms/keywords while editor handle updating the material and shader.
     */
    [Icon("Packages/me.tooster.sdf/Editor/Resources/Icons/sdf-icon-256.png")]
    [DisallowMultipleComponent]
    [GeneratePropertyBag]
    public abstract partial class Controller : MonoBehaviour, INotifyPropertyChanged {
        public delegate void StructureChangedEventHandler(Controller sender);
        // TODO: cache it, register and unregister properly
        public SdfScene SdfScene => GetComponentInParent<SdfScene>(true);

        public SdfScene.PropertyData this[PropertyPath p] => SdfScene.sceneData.controllers[this].properties[p];

        void OnTransformParentChanged() {
            OnStructureChanged();
            if (transform.parent.GetComponent<Controller>() is { } parentController)
                StructureChanged = parentController.StructureChanged;
            StructureChanged?.Invoke(this);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public event StructureChangedEventHandler? StructureChanged;

        public virtual void OnStructureChanged() => StructureChanged?.Invoke(this);

        public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, bool structural, [CallerMemberName] string? propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            if (structural) OnStructureChanged();
            else OnPropertyChanged(propertyName);
            return true;
        }

        public static void TryInstantiate<TController>(string name) where TController : Controller, new() {
            var target = Selection.activeGameObject;

            var sdf = new GameObject(name);
            var controller = sdf.AddComponent<TController>();

            if (!target) return;

            sdf.transform.SetParent(Selection.activeTransform);
        }
    }
}
