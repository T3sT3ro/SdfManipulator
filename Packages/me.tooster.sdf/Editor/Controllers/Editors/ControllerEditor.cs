using me.tooster.sdf.Editor.Controllers.SDF;
using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    [CustomEditor(typeof(Controller), true)]
    [CanEditMultipleObjects]
    public class ControllerEditor : UnityEditor.Editor {
        public override VisualElement CreateInspectorGUI() {
            var controller = (Controller)target;


            var root = new VisualElement { name = "root" };

            var editor = Resources.Load<VisualTreeAsset>("UI/controller_editor").Instantiate();
            root.Add(editor);


            if (controller.SdfScene == null)
                root.Add(new HelpBox("This node must be a descendant of a SdfScene node to work", HelpBoxMessageType.Error));

            var defaultInspector = new VisualElement();
            InspectorElement.FillDefaultInspector(defaultInspector, serializedObject, this);
            root.Add(new Label("Default inspector:"));
            root.Add(defaultInspector);

            defaultInspector.Query<PropertyField>().ForEach(
                propertyField => {
                    var capitalizedProperty = propertyField.bindingPath[0].ToString().ToUpper() + propertyField.bindingPath[1..];

                    if (!PropertyContainer.TryGetProperty(controller, new PropertyPath(capitalizedProperty), out var p))
                        return;

                    if (p.GetAttribute<ShaderPropertyAttribute>() is not null)
                        propertyField.RegisterValueChangeCallback(_ => controller.OnPropertyChanged(capitalizedProperty));

                    if (p.GetAttribute<ShaderStructuralAttribute>() is not null)
                        propertyField.RegisterValueChangeCallback(_ => controller.OnStructureChanged());
                }
            );
            return root;
        }
    }
}
