using me.tooster.sdf.Operators;
using me.tooster.sdf.Primitives;
using me.tooster.sdf.Util;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>The inspectors of scenes and controllers build their UI from the layouts in the package.</summary>
    public class InspectorTests {
        GameObject root;

        [SetUp]
        public void LoadTemplate() => root = PrefabUtility.LoadPrefabContents(SdfPaths.Templates.HelloSdfPrefab);

        [TearDown]
        public void Unload() => PrefabUtility.UnloadPrefabContents(root);

        static VisualElement InspectorOf(Object target) {
            var editor = UnityEditor.Editor.CreateEditor(target);
            try {
                return editor.CreateInspectorGUI();
            } finally {
                Object.DestroyImmediate(editor);
            }
        }

        [Test]
        public void SceneInspector_UsesItsLayout() {
            var inspector = InspectorOf(root.GetComponentInChildren<SdfScene>(true));

            Assert.That(inspector.Q<Button>("rebuild"), Is.Not.Null);
        }

        // the sphere has its own editor class, which derives from the controller editor
        [Test]
        public void ControllerInspectors_UseTheControllerLayout() {
            Assert.That(InspectorOf(root.GetComponentInChildren<SdfSphereController>(true)).Q("default-inspector"), Is.Not.Null);
            Assert.That(InspectorOf(root.GetComponentInChildren<SdfCombineController>(true)).Q("default-inspector"), Is.Not.Null);
        }
    }
}
