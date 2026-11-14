using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace me.tooster.sdf.Editor.Inspectors {
    /// <summary>The inspector of property links, which lists the shader properties of the target to choose from.</summary>
    [CustomEditor(typeof(SdfPropertyLink))]
    public class SdfPropertyLinkEditor : UnityEditor.Editor {
        public override VisualElement CreateInspectorGUI() {
            var link = (SdfPropertyLink)target;
            var root = new VisualElement();
            var targetField = new PropertyField(serializedObject.FindProperty(nameof(SdfPropertyLink.target)));
            var propertyProperty = serializedObject.FindProperty(nameof(SdfPropertyLink.property));
            var propertyField = new DropdownField("Property") { tooltip = propertyProperty.tooltip };
            propertyField.AddToClassList(BaseField<string>.alignedFieldUssClassName);
            var driverField = new PropertyField(serializedObject.FindProperty(nameof(SdfPropertyLink.driver)));
            root.Add(targetField);
            root.Add(propertyField);
            root.Add(driverField);

            void UpdateChoices() {
                propertyField.choices = ShaderPropertyNames(link.target);
                propertyField.SetValueWithoutNotify(propertyProperty.stringValue);
            }
            UpdateChoices();
            targetField.RegisterValueChangeCallback(_ => UpdateChoices());
            propertyField.RegisterValueChangedCallback(evt => {
                    propertyProperty.stringValue = evt.newValue;
                    serializedObject.ApplyModifiedProperties();
                }
            );
            return root;
        }

        internal static List<string> ShaderPropertyNames(Controller controller) {
            if (controller == null)
                return new List<string>();
            var collector = new ShaderPropertyCollector();
            PropertyContainer.Accept(collector, controller);
            return collector.ShaderProperties.Select(p => p.Property.Name).ToList();
        }
    }
}
