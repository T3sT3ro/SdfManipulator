namespace me.tooster.sdf.AST.Shaderlab {
    public class Constants {
        /// common attributes https://docs.unity3d.com/Manual/SL-Properties.html
        public enum CommonAttributes {
            Gamma, Hdr, HideInInspector, MainTexture, MainColor, NoScaleOffset, Normal, PerRendererData,
        }

        /// advanced attributes: https://docs.unity3d.com/ScriptReference/MaterialPropertyDrawer.html
        public enum AdvancedAttributes {
            Toggle, ToggleOff, Enum, KeywordEnum, KeyEnum, PowerSlider, IntRange, Space, Header,
        }
    }
}
