namespace me.tooster.sdf.AST.Foo.Syntax {
    [TokenNode("(")] public partial record OpenParenToken;
    [TokenNode(")")] public partial record CloseParenToken;
    [TokenNode("0")] public partial record ZeroLiteral;
    [TokenNode("+")] public partial record PlusToken;
    [TokenNode("-")] public partial record MinusToken;
    [TokenNode("TEST")] public partial record TestToken;
}
