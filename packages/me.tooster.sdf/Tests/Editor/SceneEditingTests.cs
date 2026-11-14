using me.tooster.sdf.Editor.Inspectors.Primitives;
using me.tooster.sdf.Primitives;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>Editing scenes through the editor's menus and handles can be undone.</summary>
    public class SceneEditingTests {
        GameObject parent;

        [SetUp]
        public void SetUp() {
            parent = new GameObject("parent");
            Selection.activeObject = null;
            Undo.IncrementCurrentGroup();
        }

        [TearDown]
        public void TearDown() {
            if (parent != null)
                Object.DestroyImmediate(parent);
        }

        [Test]
        public void CreatedPrimitive_GoesUnderTheClickedObjectAndCanBeUndone() {
            var sphere = SdfPrimitiveControllerEditor.InstantiatePrimitive<SdfSphereController>("sphere", new MenuCommand(parent));

            Assert.That(sphere.transform.parent, Is.EqualTo(parent.transform));
            Assert.That(Selection.activeGameObject, Is.EqualTo(sphere));
            Assert.That(sphere.GetComponent<SdfController>().sdfModifiers,
                Is.EqualTo(new Controller[] { sphere.GetComponent<TransformController>(), sphere.GetComponent<SdfSphereController>() }));

            Undo.PerformUndo();
            Assert.That(sphere == null, "undo doesn't remove the created object");
        }

        // the handle editors record the controller before they call its setter
        [Test]
        public void HandleChange_CanBeUndone() {
            var sphere = SdfPrimitiveControllerEditor.InstantiatePrimitive<SdfSphereController>("sphere", new MenuCommand(parent))
                .GetComponent<SdfSphereController>();
            var radius = sphere.Radius;
            Undo.IncrementCurrentGroup();

            Undo.RecordObject(sphere, "Resize sphere");
            sphere.Radius = radius + 1;
            Undo.PerformUndo();

            Assert.That(sphere.Radius, Is.EqualTo(radius));
        }
    }
}
