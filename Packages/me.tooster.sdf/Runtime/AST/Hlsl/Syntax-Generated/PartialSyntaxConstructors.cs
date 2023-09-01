// #nullable enable
// using AST.Syntax;
// using me.tooster.sdf.AST.Hlsl.Syntax;
//
// namespace me.tooster.sdf.AST.Hlsl.Syntax {
//     public partial record ArgumentList<T>                                                     { public ArgumentList() { openParenToken = new(); arguments = SeparatedList<Hlsl, T>.Empty;closeParenToken = new(); } }
//
//     public partial record BracedList<T>                                                       { public BracedList() { openBraceToken = new(); arguments = SeparatedList<Hlsl, T>.Empty; closeBraceToken = new(); } }
//
//     public partial record Semantic                                                            { public Semantic() { colonToken = new(); } }
//
//     public partial record Initializer                                                         { public Initializer() { equalsToken = new(); } }
//
//     public partial record Switch               { public partial record DefaultCase                                                         { public DefaultCase() { defaultKeyword = new(); colonToken = new(); body = new(); } } }
//
//     public partial record Break                                                               { public Break() { breakKeyword = new(); semiToken = new(); } }
//
//     public partial record ExpressionStatement                                                 { public ExpressionStatement() { semiToken = new(); } }
//
//     public partial record Discard                                                             { public Discard() { discardKeyword = new();  semiToken = new(); } }
//
//     public partial record Switch                                                              { public Switch() { switchKeyword = new(); openParen = new(); closeParen = new(); cases = new(); } }
//
//     public partial record StructDeclaration                                                   { public StructDeclaration() { semicolon = new(); } }
//
//     public partial record VariableDeclaration                                                 { public VariableDeclaration() { semiToken = new(); } }
//
//     public partial record If                   { public partial record ElseClause                                                          { public ElseClause() { elseKeyword = new(); } } }
//
//     public partial record If                                                                  { public If() { ifKeyword = new(); openParen = new(); closeParen = new(); } }
//
//     public partial record Switch               { public partial record Case                                                                { public Case() { caseKeyword = new(); colonToken = new(); body = new(); } } }
//
//     public partial record Continue                                                            { public Continue() { continueKeyword = new(); semiToken = new(); } }
//
//     public partial record Block                                                               { public Block() { openBraceToken = new(); statements = new(); closeBraceToken = new(); } }
//
//     public partial record While                                                               { public While() { whileKeyword = new(); openParen = new(); closeParen = new(); } }
//
//     public partial record Return                                                              { public Return() { returnKeyword = new(); } }
//
//     public partial record For                                                                 { public For() { forKeyword = new(); openParen = new(); firstSemiToken = new(); secondSemiToken = new(); closeParen = new(); } }
//
//     public partial record DoWhile                                                             { public DoWhile() { doKeyword = new(); openBraceToken = new(); closeBraceToken = new(); whileKeyword = new(); openParenToken = new(); closeParenToken = new(); semicolonToken = new(); } }
//
//     public partial record Typedef                                                             { public Typedef() { typedefKeyword = new(); } }
//
//     public partial record Ternary                                                             { public Ternary() { questionToken = new(); colonToken = new(); } }
//
//     public partial record Comma                                                               { public Comma() { comma = new(); } }
//
//     public partial record Indexer                                                             { public Indexer() { openBracketToken = new(); closeBracketToken = new(); } }
//
//     public partial record Cast                                                                { public Cast() { openParenToken = new(); arrayRankSpecifiers = new(); closeParenToken = new(); } }
//
//     public partial record AssignmentExpression                                                { public AssignmentExpression() { assignmentToken = new EqualsToken(); } }
//
//     public partial record Member                                                              { public Member() { dotToken = new(); } }
//
//     public partial record StructInitializer                                                   { public StructInitializer() { components = new(); } }
//
//     public partial record Parenthesized                                                       { public Parenthesized() { openParen = new(); closeParen = new(); } }
//
//     public partial record ArrayRank                                                           { public ArrayRank() { closeBracketToken = new(); } }
//
//     public partial record Type                 { public partial record Struct                                                              { public Struct() { structKeyword = new(); openBrace = new(); closeBrace = new(); } } }
//
//     public partial record Type                 { public partial record Struct                   { public partial record Member             { public Member() { semicolon = new(); } } } }
//
//     public partial record Line                                                                { public Line() { lineKeyword = new(); } }
//
//     public partial record Ifndef                                                              { public Ifndef() { ifndefKeyword = new(); } }
//
//     public partial record Undef                                                               { public Undef() { undefKeyword = new(); } }
//
//     public partial record Define                                                              { public Define() { defineKeyword = new(); argList = new() { arguments = SeparatedList<Hlsl, Identifier>.Empty }; } }
//
//     public partial record If                                                                  { public If() { ifKeyword = new(); } }
//
//     public partial record Else                                                                { public Else() { elseKeyword = new(); } }
//
//     public partial record Elif                                                                { public Elif() { elifKeyword = new(); } }
//
//     public partial record Error                                                               { public Error() { errorKeyword = new(); } }
//
//     public partial record Ifdef                                                               { public Ifdef() { ifdefKeyword = new(); } }
//
//     public partial record Endif                                                               { public Endif() { endifKeyword = new(); } }
//
//     public partial record Pragma                                                              { public Pragma() { pragmaKeyword = new(); } }
//
//     public partial record PreprocessorSyntax                                                  { public PreprocessorSyntax() { hashToken = new(); } }
// }