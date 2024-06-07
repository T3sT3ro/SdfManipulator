using me.tooster.sdf.Editor.Controllers.Data;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    public abstract class SdfPrimitiveController : Controller, IModifier<VectorData, ScalarData> {
        public abstract ScalarData Apply(VectorData input, Processor processor);
    }
}
