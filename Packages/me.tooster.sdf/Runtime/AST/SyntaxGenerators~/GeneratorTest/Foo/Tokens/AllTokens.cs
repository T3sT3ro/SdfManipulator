namespace me.tooster.sdf.AST.Foo.Tokens {
    [TokenNode("(")] public partial record OpenParenToken;
    [TokenNode(")")] public partial record CloseParenToken;
    [TokenNode("0")] public partial record ZeroLiteral;
    [TokenNode("+")] public partial record PlusToken;
    [TokenNode("-")] public partial record MinusToken;

    namespace Keywords {
        [TokenNode("TEST")] public partial record TestKeyword;
    }
}
