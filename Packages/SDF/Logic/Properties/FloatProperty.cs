using API;


namespace Logic.Properties {
    public class FloatProperty : Property<float> {
        public FloatProperty(string internalName, string displayName, float defaultValue) :
            base(internalName, displayName, defaultValue) {
            External = true;
        }

        #region selective visitor pattern

        public interface Visitor<out R> {
            public R visit(FloatProperty property);
        }

        public          R accept<R>(Visitor<R>          visitor) => visitor.visit(this);
        public override R accept<R>(Property.Visitor<R> visitor) => accept((Visitor<R>)visitor);

        #endregion
    }
}
