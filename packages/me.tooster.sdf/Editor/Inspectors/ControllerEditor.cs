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
                                propertyField.style.display = IsVisible(visibleWhenAttribute, evt.changedProperty)
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

        /// <summary>
        ///     Returns whether a field with <paramref name="visibleWhen" /> shows for the value of its driving enum field.
        ///     The attribute lists enum values, so compare them with the value, not with the position of the value in the
        ///     enum's declaration (<c>enumValueIndex</c>).
        /// </summary>
        internal static bool IsVisible(VisibleWhenAttribute visibleWhen, SerializedProperty driver)
            => visibleWhen.enumValues.Contains(driver.intValue);

        /// Finds the property that holds the attributes of a serialized field: the field itself, or a property with the name capitalized.
        static bool TryGetPropertyOfField(Controller controller, string fieldName, out IProperty property) {
            property = null;
            return !string.IsNullOrEmpty(fieldName)
             && (PropertyContainer.TryGetProperty(controller, new PropertyPath(fieldName), out property)
                 || PropertyContainer.TryGetProperty(controller, new PropertyPath(char.ToUpperInvariant(fieldName[0]) + fieldName.Substring(1)), out property));
        }

        /// <summary>
        ///     Creates an object with <typeparamref name="TController" />, e.g. for the <i>GameObject &gt; SDF</i> menu.
        /// </summary>
        public static GameObject InstantiateController<TController>(string name, MenuCommand command = null) where TController : Controller
            => CreateSdfObject(name, command, go => go.AddComponent<TController>());

        /// <summary>
        ///     Creates an object for the <i>GameObject &gt; SDF</i> menu: under the right-clicked object, otherwise under the
        ///     selected object, otherwise in the current stage, so Prefab Mode gets it instead of the main scene. The creation
        ///     can be undone, and the new object is selected.
        /// </summary>
        internal static GameObject CreateSdfObject(string name, MenuCommand command, System.Action<GameObject> addComponents) {
            var go = new GameObject(name);
            addComponents(go);
            var parent = command?.context as GameObject ?? Selection.activeGameObject;
            if (parent != null)
                GameObjectUtility.SetParentAndAlign(go, parent);
            else
                StageUtility.PlaceGameObjectInCurrentStage(go);
            GameObjectUtility.EnsureUniqueNameForSibling(go);
            Undo.RegisterCreatedObjectUndo(go, $"Create {name}");
            Undo.SetCurrentGroupName($"Create {name}");
            Selection.activeObject = go;
            return go;
        }
    }
}
