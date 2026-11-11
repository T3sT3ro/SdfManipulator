using System.Linq;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace me.tooster.sdf.Editor.Inspectors {
    /// <summary>
    ///     The inspector of all controllers: the default inspector, with structural fields marked and disabled outside
    ///     Prefab Mode, and fields that <see cref="VisibleWhenAttribute" /> hides.
    /// </summary>
    /// <remarks>
    ///     Changes don't go through this inspector. <see cref="Controller" />.OnValidate sets the uniforms and detects
    ///     structural changes for edits from any source, including undo and editing several objects at once. To find the
    ///     attributes of a serialized field, the inspector looks for a property with the same name, then for one with the
    ///     name capitalized, e.g. the field <c>radius</c> and the property <c>Radius</c>. A field without such a property
    ///     still works, but the inspector doesn't mark it.
    /// </remarks>
    [CustomEditor(typeof(Controller), true)]
    [CanEditMultipleObjects]
    public class ControllerEditor : UnityEditor.Editor {
        const string STRUCTURAL_TOOLTIP = "Structural property regenerating a shader. Editable only in a prefab stage.";

        public override VisualElement CreateInspectorGUI() {
            var controller = (Controller)target;


            var root = new VisualElement { name = "root" };

            var editor = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(SdfPaths.UI.ControllerEditor).Instantiate();
            root.Add(editor);


            if (controller.SdfScene == null)
                root.Add(new HelpBox("This node must be a descendant of a SdfScene node to work", HelpBoxMessageType.Error));

            var defaultInspector = editor.Q<VisualElement>("default-inspector");
            InspectorElement.FillDefaultInspector(defaultInspector, serializedObject, this);

            defaultInspector.Query<PropertyField>().ForEach(propertyField => {
                    if (!TryGetPropertyOfField(controller, propertyField.bindingPath, out var p))
                        return;

                    if (p.GetAttribute<ShaderPropertyAttribute>() is not null)
                        propertyField.AddToClassList("shader-property");

                    if (p.GetAttribute<ShaderStructuralAttribute>() is not null) {
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
                        driverEnumPropertyField.RegisterValueChangeCallback(evt => {
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

        /// Finds the property that holds the attributes of a serialized field: the field itself, or a property with the name capitalized.
        static bool TryGetPropertyOfField(Controller controller, string fieldName, out IProperty property) {
            property = null;
            return !string.IsNullOrEmpty(fieldName)
             && (PropertyContainer.TryGetProperty(controller, new PropertyPath(fieldName), out property)
                 || PropertyContainer.TryGetProperty(controller, new PropertyPath(char.ToUpperInvariant(fieldName[0]) + fieldName.Substring(1)), out property));
        }

        public static void InstantiateController<TController>(string name) where TController : Controller, new() {
            var target = Selection.activeGameObject;

            var sdf = new GameObject(name);
            ObjectNames.SetNameSmart(sdf, name);
            if (target)
                sdf.transform.SetParent(Selection.activeTransform);
            sdf.transform.localPosition = Vector3.zero;
            sdf.AddComponent<TController>();
        }
    }
}
