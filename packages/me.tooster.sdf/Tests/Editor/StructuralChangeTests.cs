using System.Linq;
using me.tooster.sdf.Operators;
using me.tooster.sdf.Util;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>
    ///     Edits that bypass the property setters, like the inspector's, regenerate the shader only when they change the
    ///     value of a structural property.
    /// </summary>
    public class StructuralChangeTests {
        GameObject           root;
        SdfScene             scene;
        SdfCombineController combine;

        [SetUp]
        public void LoadTemplate() {
            root = PrefabUtility.LoadPrefabContents(SdfPaths.Templates.HelloSdfPrefab);
            scene = root.GetComponentInChildren<SdfScene>(true);
            combine = root.GetComponentInChildren<SdfCombineController>(true);
            scene.RevalidateScene();
            scene.RequiresRegeneration = false;
            scene.RequiresUpdate = false;
        }

        [TearDown]
        public void Unload() => PrefabUtility.UnloadPrefabContents(root);

        /// Changes a serialized field the way the inspector does, which calls OnValidate.
        static void Edit(Object target, string field, System.Action<SerializedProperty> change) {
            var serializedObject = new SerializedObject(target);
            change(serializedObject.FindProperty(field));
            serializedObject.ApplyModifiedProperties();
        }

        [Test]
        public void StructuralFieldEdit_Regenerates() {
            Edit(combine, "operation", p => p.enumValueIndex = (p.enumValueIndex + 1) % p.enumNames.Length);

            Assert.That(scene.RequiresRegeneration, Is.True);
        }

        [Test]
        public void UniformFieldEdit_OnlyUpdatesUniforms() {
            Edit(combine, "blendFactor", p => p.floatValue += 0.25f);

            Assert.That(scene.RequiresRegeneration, Is.False);
            Assert.That(scene.RequiresUpdate, Is.True);
        }

        [Test]
        public void ModifierListEdit_Regenerates() {
            var sdf = root.GetComponentsInChildren<SdfController>(true).First(c => c.sdfModifiers.Length > 0);

            Edit(sdf, nameof(SdfController.sdfModifiers), p => p.arraySize = 0);

            Assert.That(scene.RequiresRegeneration, Is.True);
        }

        [Test]
        public void SetterChange_IsNotReportedAgainByTheNextEdit() {
            combine.SingleId = !combine.SingleId;
            Assert.That(scene.RequiresRegeneration, Is.True);
            scene.RequiresRegeneration = false;

            Edit(combine, "blendFactor", p => p.floatValue += 0.25f);

            Assert.That(scene.RequiresRegeneration, Is.False);
        }
    }
}
