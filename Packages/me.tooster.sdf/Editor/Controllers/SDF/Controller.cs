#nullable enable
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using Component = UnityEngine.Component;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    /**
     * TODO: refactor to a editor + runtime type with conditional compilation.
     * Runtime would handle updating uniforms/keywords while editor handle updating the material and shader.
     */
    [Icon("Packages/me.tooster.sdf/Editor/Resources/Icons/sdf-icon-256.png")]
    public abstract class Controller : MonoBehaviour, INotifyPropertyChanged {
        public delegate void StructureChangedEventHandler(Controller sender);
        // TODO: cache it, register and unregister properly
        public SdfScene SdfScene => GetComponentInParent<SdfScene>(true);

        public SdfScene.PropertyData this[PropertyPath p] => SdfScene.sceneData.controllers[this].properties[p];

        void OnTransformParentChanged() {
            OnStructureChanged(); // notify currently subscribed parent
            if (transform.GetComponentInParent<Controller>() is { } parentController)
                StructureChanged = parentController.StructureChanged; // clear all previous listeners as well
            StructureChanged?.Invoke(this);
        }

        public event PropertyChangedEventHandler?  PropertyChanged;
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
            if (target) sdf.transform.SetParent(Selection.activeTransform);
            sdf.transform.localPosition = Vector3.zero;

            var controller = sdf.AddComponent<TController>();
            ObjectNames.SetNameSmart(sdf, name);
        }

        public T GetNextControllerInStack<T>() {
            var thisIndex = GetComponentIndex();
            return GetComponents<T>().First(c => (c as Component)!.GetComponentIndex() > thisIndex);
        }

        public T GetPreviousControllerInStack<T>() {
            var thisIndex = GetComponentIndex();
            return GetComponents<T>().First(c => (c as Component)!.GetComponentIndex() > thisIndex);
        }
    }
}
