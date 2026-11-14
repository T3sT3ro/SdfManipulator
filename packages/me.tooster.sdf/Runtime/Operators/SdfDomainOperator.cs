#nullable enable
using System.Linq;
using me.tooster.sdf.Data;
using Type = System.Type;

namespace me.tooster.sdf.Operators {
    /// <summary>
    ///     A modifier of the SDF stack that changes the point at which the modifiers after it evaluate, e.g. repeating or
    ///     mirroring the space. Adding the component puts it into the object's stack right after the transform, so the
    ///     operation follows the object.
    /// </summary>
    public abstract class SdfDomainOperator : Controller, IModifier<VectorData, VectorData> {
        public abstract VectorData Apply(VectorData input, Processor processor);

        public override IData Apply(IData input, Processor processor) => Apply((VectorData)input, processor);
        public override Type  GetInputType()                          => typeof(VectorData);
        public override Type  GetOutputType()                         => typeof(VectorData);

        // Unity calls Reset when the component is added: join the SDF stack after the transform, before the primitive
        protected virtual void Reset() {
            if (!TryGetComponent<SdfController>(out var sdf) || sdf.sdfModifiers.Contains(this))
                return;
            var modifiers = sdf.sdfModifiers.ToList();
            modifiers.Insert(modifiers.FindIndex(m => m is TransformController) + 1, this);
            sdf.sdfModifiers = modifiers.ToArray();
            sdf.OnStructureChanged();
        }
    }
}
