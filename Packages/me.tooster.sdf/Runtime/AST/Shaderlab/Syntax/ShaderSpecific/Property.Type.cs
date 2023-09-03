using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific {
    public partial record Property {
        [Syntax] public abstract partial record Type : Syntax<Shaderlab>;

        [Syntax] public partial record PredefinedType : Type {
            // Integer, Float, Texture2D, Texture2DArray, Texture3D, Cubemap, CubemapArray, Color, Vector
            private readonly TypeKeyword _type;
        }

        // Range(0.0, 1.0) is the same as Float in material properties
        [Syntax] public partial record RangeType : Type {
            [Init] private readonly RangeKeyword    _rangeKeyword;
            [Init] private readonly OpenParenToken  _openParenToken;
            private readonly        FloatLiteral    _min;
            [Init] private readonly CommaToken      _commaToken;
            private readonly        FloatLiteral    _max;
            [Init] private readonly CloseParenToken _closeParenToken;
        }
    }
}
