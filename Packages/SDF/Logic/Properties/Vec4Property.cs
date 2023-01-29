using API;
using UnityEngine;

namespace Logic.Properties {
    public class Vec4Property : Property<Vector4> {
        public Vec4Property(string internalName, string displayName, Vector4 defaultValue) :
            base(internalName, displayName, defaultValue) {
            External = true;
        }

        #region selective visitor pattern

        public interface Visitor<out R> {
            public R visit(Vec4Property property);
        }

        public          R accept<R>(Visitor<R>          visitor) => visitor.visit(this);
        public override R accept<R>(Property.Visitor<R> visitor) => accept((Visitor<R>)visitor);

        #endregion
    }
}
