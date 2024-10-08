using System.Linq;
using me.tooster.sdf.Editor.Controllers.SDF;
using Unity.Properties;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    /**
     * A base controller editor which binds serialized fields to events for triggering uniform updates and shader regenerations.
     * Prerequisites for Controller to make the editor work as intended:
     * <list type="bullet">
     * <item>controller has serialized fields and reactive properties (i.e. ones that trigget SetField and OnPropertyChanged)</item>
     * <item>the serialized field and the reactive property have the same name, but the property is capitalized e.g. field 'size' and property 'Size'</item>
     * <item>properties are annotated with either [ShaderProperty] (for uniforms or [ShaderStructural] (for regenerating with new definitions)</item>
     * </list>
     */
    [CustomEditor(typeof(Controller), true)]
    [CanEditMultipleObjects]
    public class ControllerEditor : UnityEditor.Editor {
        const string UI_CONTROLLER_EDITOR_RESOURCE = "UI/controller_editor";
        const string STRUCTURAL_TOOLTIP            = "Structural property regenerating a shader. Editable only in a prefab stage.";

        public override VisualElement CreateInspectorGUI() {
            var controller = (Controller)target;


            var root = new VisualElement { name = "root" };

            var editor = Resources.Load<VisualTreeAsset>(UI_CONTROLLER_EDITOR_RESOURCE).Instantiate();
            root.Add(editor);


            if (controller.SdfScene == null)
                root.Add(new HelpBox("This node must be a descendant of a SdfScene node to work", HelpBoxMessageType.Error));

            var defaultInspector = editor.Q<VisualElement>("default-inspector");
            InspectorElement.FillDefaultInspector(defaultInspector, serializedObject, this);

            defaultInspector.Query<PropertyField>().ForEach(
                propertyField => {
                    // this is bound to a field
                    var propertyPath = propertyField.bindingPath;
                    // implicit assumption: a controller has a reactive property with the same name, but capitalized. That property triggers events
                    var capitalizedProperty = propertyPath.Substring(0, 1).ToUpper() + propertyPath.Substring(1);

                    if (!PropertyContainer.TryGetProperty(controller, new PropertyPath(capitalizedProperty), out var p))
                        return;

                    if (p.GetAttribute<ShaderPropertyAttribute>() is not null) {
                        // A runtime proeprty can be updated at any time
                        propertyField.RegisterValueChangeCallback(_ => controller.OnPropertyChanged(capitalizedProperty));
                        propertyField.AddToClassList("shader-property");
                    }

                    // TrackPropertyValue is here required, because the change event is triggered when inspector opens for regular property field
                    // this caused regenerations every time the selected object changed 
                    if (p.GetAttribute<ShaderStructuralAttribute>() is not null) {
                        propertyField.TrackPropertyValue(serializedObject.FindProperty(propertyPath), _ => controller.OnStructureChanged());
                        // add 'structural' USS class to property field
                        propertyField.AddToClassList("shader-structural");
                        propertyField.tooltip = propertyField.tooltip is null or ""
                            ? STRUCTURAL_TOOLTIP
                            : propertyField.tooltip + $"\n{STRUCTURAL_TOOLTIP}";
                        if (PrefabStageUtility.GetPrefabStage(controller.gameObject) == null)
                            propertyField.SetEnabled(false);
                    }

                    // track property visibility if it depends on some enum value of a backing field
                    // for example an int property "Layers" in onion skin operator depends on property Variant with enum value "LAYERS"
                    // this means we should RegisterValueChangeCallback when the enum value changes in Variant and hide/show the Layers property using initial display style or None
                    void addConoditionalHooks(VisibleWhenAttribute visibleWhenAttribute) {
                        var driverEnumSerializedProperty = serializedObject.FindProperty(visibleWhenAttribute.enumBackingField);
                        var driverEnumPropertyField =
                            defaultInspector.Q<PropertyField>("PropertyField:" + driverEnumSerializedProperty.propertyPath);
                        var initialDisplayStyle = driverEnumPropertyField.style.display;

                        // depends on the implicit change event when a property field is instantiated
                        driverEnumPropertyField.RegisterValueChangeCallback(
                            evt => {
                                propertyField.style.display =
                                    visibleWhenAttribute.enumValues.Contains(evt.changedProperty.enumValueIndex)
                                        ? initialDisplayStyle
                                        : DisplayStyle.None;
                            }
                        );
                    }

                    if (p.GetAttribute<VisibleWhenAttribute>() is { } dependsOn)
                        addConoditionalHooks(dependsOn);
                }
            );
            return root;
        }

        public static void InstantiateController<TController>(string name) where TController : Controller, new() {
            var target = Selection.activeGameObject;

            var sdf = new GameObject(name);
            ObjectNames.SetNameSmart(sdf, name);
            if (target) sdf.transform.SetParent(Selection.activeTransform);
            sdf.transform.localPosition = Vector3.zero;
            sdf.AddComponent<TController>();
        }
    }
}
