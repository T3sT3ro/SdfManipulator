#nullable enable
using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace me.tooster.sdf.Editor.API {
    // TODO: replace with observable properties on types and property metadata (to avoid duplication and decouple concepts)
    // TODO: declare properties getter as dictionary of properties (in controllers). Consider removing internal name? Or not? (user defined name?)
    /// <summary>
    /// A wrapper around a 
    /// A constant literal or a uniform, which can be exposed to outside world.
    /// They are like uniforms and blackboard properties in shader graph
    /// </summary>
    [Obsolete]
    public abstract class Property {
        protected Property(string InternalName, string DisplayName) {
            this.InternalName = InternalName;
            this.DisplayName = DisplayName;
        }

        [CreateProperty] public bool IsExposed { get; set; }

        [CreateProperty] public string InternalName { get; init; }
        [CreateProperty] public string DisplayName  { get; init; }

        public event Action<Property>? onPropertyChanged;

        public void NotifyPropertyChanged() => onPropertyChanged?.Invoke(this);
    }



    /// <summary>Typed property with possibility to export to external world</summary>
    /// <typeparam name="T">type that this property holds</typeparam>
    [Serializable]
    public class Property<T> : Property {
        [SerializeField] T propertyValue;

        public Property(string InternalName, string DisplayName, T DefaultValue) : base(InternalName, DisplayName)
            => this.DefaultValue = propertyValue = DefaultValue;

        [CreateProperty] public T DefaultValue { get; private set; }

        public T Value {
            get => propertyValue;
            set {
                if (EqualityComparer<T>.Default.Equals(propertyValue, value)) return;
                propertyValue = value;
                NotifyPropertyChanged();
            }
        }

        public override string ToString() => $"{DisplayName} ({InternalName}<{typeof(T)}>: {Value})";

        public static implicit operator T(Property<T> self) => self.Value;
    }



    [CustomPropertyDrawer(typeof(Property), true)]
    public class ShaderPropertyDrawer : PropertyDrawer {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            var asset = Resources.Load<VisualTreeAsset>("UI/shader_property_drawer");
            var drawer = asset.Instantiate(property.propertyPath);
            drawer.Q<Label>().name = property.FindPropertyRelative("DisplayName").stringValue;
            // drawer.Q<PropertyField>("ID").BindProperty(property.FindPropertyRelative("InternalName"));
            return drawer;
        }
    }
}
