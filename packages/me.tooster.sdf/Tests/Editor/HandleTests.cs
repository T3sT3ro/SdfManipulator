using me.tooster.sdf.Editor.Inspectors.Primitives;
using NUnit.Framework;
using UnityEngine;

namespace me.tooster.sdf.Tests.Editor {
    /// <summary>The values that the scene handles of primitives compute from the position of a dragged handle.</summary>
    public class HandleTests {
        // the capped torus keeps the cap on both sides of the z axis, so each end of the arc sets the cap on its own side
        [TestCase(1, 0, 1, 1, 0, Mathf.PI / 4)]
        [TestCase(-1, 0, 1, -1, 0, Mathf.PI / 4)]
        [TestCase(1, 0, -1, 1, 0, 3 * Mathf.PI / 4)]
        [TestCase(0, 0, -1, -1, 0, Mathf.PI)]
        // the y coordinate doesn't matter
        [TestCase(1, 5, 0, 1, 0, Mathf.PI / 2)]
        // past the z axis on the other side, the cap stops at the end that it came from
        [TestCase(-0.1f, 0, 1, 1, 0.2f, 0)]
        [TestCase(-0.1f, 0, -1, 1, 3f, Mathf.PI)]
        public void TorusCap_PutsTheEndOfTheArcUnderTheCursor(float x, float y, float z, float side, float previousCap, float expected) {
            Assert.That(SdfTorusControllerEditor.CapAt(new Vector3(x, y, z), side, previousCap), Is.EqualTo(expected).Within(1e-5f));
        }
    }
}
