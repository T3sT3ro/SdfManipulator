// #nullable enable
// using AST.Syntax;
// using me.tooster.sdf.AST.Hlsl.Syntax;
//
// namespace me.tooster.sdf.AST.Hlsl.Syntax { /*./Identifier.cs:                                 */ public partial record Identifier                                                          { public IdentifierToken id { get => _id; init => _id                   = value with { Parent = this }; } }
//
// /*./Lists.cs:                                      */ public partial record ArgumentList<T>                                                     { public OpenParenToken openParenToken { get => _openParenToken; init => _openParenToken       = value with { Parent = this }; } }
//
// /*./Lists.cs:                                      */ public partial record ArgumentList<T>                                                     { public SeparatedList<Hlsl, T> arguments { get => _arguments; init => _arguments            = value with { Parent = this }; } }
//
// /*./Lists.cs:                                      */ public partial record ArgumentList<T>                                                     { public CloseParenToken closeParenToken { get => _closeParenToken; init => _closeParenToken      = value with { Parent = this }; } }
//
// /*./Lists.cs:                                      */ public partial record BracedList<T>                                                       { public OpenBraceToken openBraceToken { get => _openBraceToken; init => _openBraceToken       = value with { Parent = this }; } }
//
// /*./Lists.cs:                                      */ public partial record BracedList<T>                                                       { public SeparatedList<Hlsl, T> arguments { get => _arguments; init => _arguments            = value with { Parent = this }; } }
//
// /*./Lists.cs:                                      */ public partial record BracedList<T>                                                       { public CloseBraceToken closeBraceToken { get => _closeBraceToken; init => _closeBraceToken      = value with { Parent = this }; } }
//
// /*./Semantic.cs:                                   */ public partial record Semantic                                                            { public ColonToken colonToken { get => _colonToken; init => _colonToken           = value with { Parent = this }; } }
//
// /*./Semantic.cs:                                   */ public partial record Semantic                                                            { public SemanticToken semanticToken { get => _semanticToken; init => _semanticToken        = value with { Parent = this }; } }
//
// /*./Statements/Initializer.cs:                     */ public partial record Initializer                                                         { public EqualsToken equalsToken { get => _equalsToken; init => _equalsToken          = value with { Parent = this }; } }
//
// /*./Statements/Initializer.cs:                     */ public partial record Initializer                                                         { public Expression value { get => _value; init => _value                = value with { Parent = this }; } }
//
//     public partial record Switch               { /*./Statements/Switch.DefaultCase.cs:              */ public partial record DefaultCase                                                         { public DefaultKeyword defaultKeyword { get => _defaultKeyword; init => _defaultKeyword       = value with { Parent = this }; } } }
//
//     public partial record Switch               { /*./Statements/Switch.DefaultCase.cs:              */ public partial record DefaultCase                                                         { public ColonToken colonToken { get => _colonToken; init => _colonToken           = value with { Parent = this }; } } }
//
//     public partial record Switch               { /*./Statements/Switch.DefaultCase.cs:              */ public partial record DefaultCase                                                         { public SyntaxList<Hlsl, Statement> body { get => _body; init => _body                 = value with { Parent = this }; } } }
//
// /*./Statements/Break.cs:                           */ public partial record Break                                                               { public BreakKeyword breakKeyword { get => _breakKeyword; init => _breakKeyword         = value with { Parent = this }; } }
//
// /*./Statements/Break.cs:                           */ public partial record Break                                                               { public SemiToken semiToken { get => _semiToken; init => _semiToken            = value with { Parent = this }; } }
//
// /*./Statements/ExpressionStatement.cs:             */ public partial record ExpressionStatement                                                 { public Expression? expression { get => _expression; init => _expression           = value with { Parent = this }; } }
//
// /*./Statements/ExpressionStatement.cs:             */ public partial record ExpressionStatement                                                 { public SemiToken semiToken { get => _semiToken; init => _semiToken            = value with { Parent = this }; } }
//
// /*./Statements/Discard.cs:                         */ public partial record Discard                                                             { public DiscardKeyword discardKeyword { get => _discardKeyword; init => _discardKeyword       = value with { Parent = this }; } }
//
// /*./Statements/Discard.cs:                         */ public partial record Discard                                                             { public SemiToken semiToken { get => _semiToken; init => _semiToken            = value with { Parent = this }; } }
//
//     public partial record For                  { /*./Statements/For.VariableInitializer.cs:         */ public partial record VariableInitializer                                                 { public SeparatedList<Hlsl, AssignmentExpression> initializers { get => _initializers; init => _initializers         = value with { Parent = this }; } } }
//
// /*./Statements/Switch.cs:                          */ public partial record Switch                                                              { public SwitchKeyword switchKeyword { get => _switchKeyword; init => _switchKeyword        = value with { Parent = this }; } }
//
// /*./Statements/Switch.cs:                          */ public partial record Switch                                                              { public OpenParenToken openParen { get => _openParen; init => _openParen            = value with { Parent = this }; } }
//
// /*./Statements/Switch.cs:                          */ public partial record Switch                                                              { public Identifier selector { get => _selector; init => _selector             = value with { Parent = this }; } }
//
// /*./Statements/Switch.cs:                          */ public partial record Switch                                                              { public CloseParenToken closeParen { get => _closeParen; init => _closeParen           = value with { Parent = this }; } }
//
// /*./Statements/Switch.cs:                          */ public partial record Switch                                                              { public SyntaxList<Hlsl, Switch.Case> cases { get => _cases; init => _cases                = value with { Parent = this }; } }
//
// /*./Statements/Declarations/StructDeclaration.cs:  */ public partial record StructDeclaration                                                   { public Type.Struct shape { get => _shape; init => _shape                = value with { Parent = this }; } }
//
// /*./Statements/Declarations/StructDeclaration.cs:  */ public partial record StructDeclaration                                                   { public SemiToken semicolon { get => _semicolon; init => _semicolon            = value with { Parent = this }; } }
//
// /*./Statements/Declarations/VariableDeclaration.cs:*/ public partial record VariableDeclaration                                                 { public VariableDeclarator declarator { get => _declarator; init => _declarator           = value with { Parent = this }; } }
//
// /*./Statements/Declarations/VariableDeclaration.cs:*/ public partial record VariableDeclaration                                                 { public SemiToken semiToken { get => _semiToken; init => _semiToken            = value with { Parent = this }; } }
//
// /*./Statements/Declarations/FunctionDeclaration.cs:*/ public partial record FunctionDeclaration                                                 { public Type returnType { get => _returnType; init => _returnType           = value with { Parent = this }; } }
//
// /*./Statements/Declarations/FunctionDeclaration.cs:*/ public partial record FunctionDeclaration                                                 { public Identifier id { get => _id; init => _id                   = value with { Parent = this }; } }
//
// /*./Statements/Declarations/FunctionDeclaration.cs:*/ public partial record FunctionDeclaration                                                 { public ArgumentList<FunctionDeclaration.Parameter> paramList { get => _paramList; init => _paramList            = value with { Parent = this }; } }
//
// /*./Statements/Declarations/FunctionDeclaration.cs:*/ public partial record FunctionDeclaration                                                 { public Semantic? returnSemantic { get => _returnSemantic; init => _returnSemantic       = value with { Parent = this }; } }
//
// /*./Statements/Declarations/FunctionDeclaration.cs:*/ public partial record FunctionDeclaration                                                 { public Block body { get => _body; init => _body                 = value with { Parent = this }; } }
//
//     public partial record FunctionDeclaration  { /*./Statements/Declarations/FunctionDeclaration.cs:*/ public partial record Parameter                                                           { public Token<Hlsl>? modifier { get => _modifier; init => _modifier             = value with { Parent = this }; } } }
//
//     public partial record FunctionDeclaration  { /*./Statements/Declarations/FunctionDeclaration.cs:*/ public partial record Parameter                                                           { public Type type { get => _type; init => _type                 = value with { Parent = this }; } } }
//
//     public partial record FunctionDeclaration  { /*./Statements/Declarations/FunctionDeclaration.cs:*/ public partial record Parameter                                                           { public Identifier id { get => _id; init => _id                   = value with { Parent = this }; } } }
//
//     public partial record FunctionDeclaration  { /*./Statements/Declarations/FunctionDeclaration.cs:*/ public partial record Parameter                                                           { public Semantic? semantic { get => _semantic; init => _semantic             = value with { Parent = this }; } } }
//
//     public partial record FunctionDeclaration  { /*./Statements/Declarations/FunctionDeclaration.cs:*/ public partial record Parameter                                                           { public Expression? initializer { get => _initializer; init => _initializer          = value with { Parent = this }; } } }
//
//     public partial record If                   { /*./Statements/If.ElseClause.cs:                   */ public partial record ElseClause                                                          { public ElseKeyword elseKeyword { get => _elseKeyword; init => _elseKeyword          = value with { Parent = this }; } } }
//
//     public partial record If                   { /*./Statements/If.ElseClause.cs:                   */ public partial record ElseClause                                                          { public Statement statement { get => _statement; init => _statement            = value with { Parent = this }; } } }
//
// /*./Statements/If.cs:                              */ public partial record If                                                                  { public IfKeyword ifKeyword { get => _ifKeyword; init => _ifKeyword            = value with { Parent = this }; } }
//
// /*./Statements/If.cs:                              */ public partial record If                                                                  { public OpenParenToken openParen { get => _openParen; init => _openParen            = value with { Parent = this }; } }
//
// /*./Statements/If.cs:                              */ public partial record If                                                                  { public Expression test { get => _test; init => _test                 = value with { Parent = this }; } }
//
// /*./Statements/If.cs:                              */ public partial record If                                                                  { public CloseParenToken closeParen { get => _closeParen; init => _closeParen           = value with { Parent = this }; } }
//
// /*./Statements/If.cs:                              */ public partial record If                                                                  { public Statement then { get => _then; init => _then                 = value with { Parent = this }; } }
//
// /*./Statements/If.cs:                              */ public partial record If                                                                  { public If.ElseClause? elseClause { get => _elseClause; init => _elseClause           = value with { Parent = this }; } }
//
//     public partial record Switch               { /*./Statements/Switch.Case.cs:                     */ public partial record Case                                                                { public CaseKeyword caseKeyword { get => _caseKeyword; init => _caseKeyword          = value with { Parent = this }; } } }
//
//     public partial record Switch               { /*./Statements/Switch.Case.cs:                     */ public partial record Case                                                                { public IntLiteral label { get => _label; init => _label                = value with { Parent = this }; } } }
//
//     public partial record Switch               { /*./Statements/Switch.Case.cs:                     */ public partial record Case                                                                { public ColonToken colonToken { get => _colonToken; init => _colonToken           = value with { Parent = this }; } } }
//
//     public partial record Switch               { /*./Statements/Switch.Case.cs:                     */ public partial record Case                                                                { public SyntaxList<Hlsl, Statement> body { get => _body; init => _body                 = value with { Parent = this }; } } }
//
// /*./Statements/Continue.cs:                        */ public partial record Continue                                                            { public ContinueKeyword continueKeyword { get => _continueKeyword; init => _continueKeyword      = value with { Parent = this }; } }
//
// /*./Statements/Continue.cs:                        */ public partial record Continue                                                            { public SemiToken semiToken { get => _semiToken; init => _semiToken            = value with { Parent = this }; } }
//
// /*./Statements/Block.cs:                           */ public partial record Block                                                               { public OpenBraceToken openBraceToken { get => _openBraceToken; init => _openBraceToken       = value with { Parent = this }; } }
//
// /*./Statements/Block.cs:                           */ public partial record Block                                                               { public SyntaxList<Hlsl, Statement> statements { get => _statements; init => _statements           = value with { Parent = this }; } }
//
// /*./Statements/Block.cs:                           */ public partial record Block                                                               { public CloseBraceToken closeBraceToken { get => _closeBraceToken; init => _closeBraceToken      = value with { Parent = this }; } }
//
// /*./Statements/While.cs:                           */ public partial record While                                                               { public WhileKeyword whileKeyword { get => _whileKeyword; init => _whileKeyword         = value with { Parent = this }; } }
//
// /*./Statements/While.cs:                           */ public partial record While                                                               { public OpenParenToken openParen { get => _openParen; init => _openParen            = value with { Parent = this }; } }
//
// /*./Statements/While.cs:                           */ public partial record While                                                               { public Expression test { get => _test; init => _test                 = value with { Parent = this }; } }
//
// /*./Statements/While.cs:                           */ public partial record While                                                               { public CloseParenToken closeParen { get => _closeParen; init => _closeParen           = value with { Parent = this }; } }
//
// /*./Statements/While.cs:                           */ public partial record While                                                               { public Statement body { get => _body; init => _body                 = value with { Parent = this }; } }
//
// /*./Statements/Return.cs:                          */ public partial record Return                                                              { public ReturnKeyword returnKeyword { get => _returnKeyword; init => _returnKeyword        = value with { Parent = this }; } }
//
// /*./Statements/Return.cs:                          */ public partial record Return                                                              { public Expression? expression { get => _expression; init => _expression           = value with { Parent = this }; } }
//
// /*./Statements/For.cs:                             */ public partial record For                                                                 { public ForKeyword forKeyword { get => _forKeyword; init => _forKeyword           = value with { Parent = this }; } }
//
// /*./Statements/For.cs:                             */ public partial record For                                                                 { public OpenParenToken openParen { get => _openParen; init => _openParen            = value with { Parent = this }; } }
//
// /*./Statements/For.cs:                             */ public partial record For                                                                 { public Syntax.Statements.For.Initializer? initializer { get => _initializer; init => _initializer          = value with { Parent = this }; } }
//
// /*./Statements/For.cs:                             */ public partial record For                                                                 { public SemiToken firstSemiToken { get => _firstSemiToken; init => _firstSemiToken       = value with { Parent = this }; } }
//
// /*./Statements/For.cs:                             */ public partial record For                                                                 { public Expression? condition { get => _condition; init => _condition            = value with { Parent = this }; } }
//
// /*./Statements/For.cs:                             */ public partial record For                                                                 { public SemiToken secondSemiToken { get => _secondSemiToken; init => _secondSemiToken      = value with { Parent = this }; } }
//
// /*./Statements/For.cs:                             */ public partial record For                                                                 { public SeparatedList<Hlsl, Expression>? increments { get => _increments; init => _increments           = value with { Parent = this }; } }
//
// /*./Statements/For.cs:                             */ public partial record For                                                                 { public CloseParenToken closeParen { get => _closeParen; init => _closeParen           = value with { Parent = this }; } }
//
// /*./Statements/For.cs:                             */ public partial record For                                                                 { public Statement body { get => _body; init => _body                 = value with { Parent = this }; } }
//
// /*./Statements/DoWhile.cs:                         */ public partial record DoWhile                                                             { public DoKeyword doKeyword { get => _doKeyword; init => _doKeyword            = value with { Parent = this }; } }
//
// /*./Statements/DoWhile.cs:                         */ public partial record DoWhile                                                             { public OpenBraceToken openBraceToken { get => _openBraceToken; init => _openBraceToken       = value with { Parent = this }; } }
//
// /*./Statements/DoWhile.cs:                         */ public partial record DoWhile                                                             { public Statement body { get => _body; init => _body                 = value with { Parent = this }; } }
//
// /*./Statements/DoWhile.cs:                         */ public partial record DoWhile                                                             { public CloseBraceToken closeBraceToken { get => _closeBraceToken; init => _closeBraceToken      = value with { Parent = this }; } }
//
// /*./Statements/DoWhile.cs:                         */ public partial record DoWhile                                                             { public WhileKeyword whileKeyword { get => _whileKeyword; init => _whileKeyword         = value with { Parent = this }; } }
//
// /*./Statements/DoWhile.cs:                         */ public partial record DoWhile                                                             { public OpenParenToken openParenToken { get => _openParenToken; init => _openParenToken       = value with { Parent = this }; } }
//
// /*./Statements/DoWhile.cs:                         */ public partial record DoWhile                                                             { public Expression test { get => _test; init => _test                 = value with { Parent = this }; } }
//
// /*./Statements/DoWhile.cs:                         */ public partial record DoWhile                                                             { public CloseParenToken closeParenToken { get => _closeParenToken; init => _closeParenToken      = value with { Parent = this }; } }
//
// /*./Statements/DoWhile.cs:                         */ public partial record DoWhile                                                             { public SemiToken semicolonToken { get => _semicolonToken; init => _semicolonToken       = value with { Parent = this }; } }
//
// /*./Statements/Typedef.cs:                         */ public partial record Typedef                                                             { public TypedefKeyword typedefKeyword { get => _typedefKeyword; init => _typedefKeyword       = value with { Parent = this }; } }
//
// /*./Statements/Typedef.cs:                         */ public partial record Typedef                                                             { public Type type { get => _type; init => _type                 = value with { Parent = this }; } }
//
// /*./Statements/Typedef.cs:                         */ public partial record Typedef                                                             { public Identifier id { get => _id; init => _id                   = value with { Parent = this }; } }
//
// /*./Expressions/LiteralExpression.cs:              */ public partial record LiteralExpression<T>                                                { public T literal { get => _literal; init => _literal              = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Ternary.cs:              */ public partial record Ternary                                                             { public Expression condition { get => _condition; init => _condition            = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Ternary.cs:              */ public partial record Ternary                                                             { public QuestionToken questionToken { get => _questionToken; init => _questionToken        = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Ternary.cs:              */ public partial record Ternary                                                             { public Expression whenTrue { get => _whenTrue; init => _whenTrue             = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Ternary.cs:              */ public partial record Ternary                                                             { public ColonToken colonToken { get => _colonToken; init => _colonToken           = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Ternary.cs:              */ public partial record Ternary                                                             { public Expression whenFalse { get => _whenFalse; init => _whenFalse            = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Comma.cs:                */ public partial record Comma                                                               { public Expression left { get => _left; init => _left                 = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Comma.cs:                */ public partial record Comma                                                               { public CommaToken comma { get => _comma; init => _comma                = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Comma.cs:                */ public partial record Comma                                                               { public Expression right { get => _right; init => _right                = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Indexer.cs:              */ public partial record Indexer                                                             { public Expression expression { get => _expression; init => _expression           = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Indexer.cs:              */ public partial record Indexer                                                             { public OpenBracketToken openBracketToken { get => _openBracketToken; init => _openBracketToken     = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Indexer.cs:              */ public partial record Indexer                                                             { public Expression index { get => _index; init => _index                = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Indexer.cs:              */ public partial record Indexer                                                             { public CloseBracketToken closeBracketToken { get => _closeBracketToken; init => _closeBracketToken    = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Cast.cs:                 */ public partial record Cast                                                                { public OpenParenToken openParenToken { get => _openParenToken; init => _openParenToken       = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Cast.cs:                 */ public partial record Cast                                                                { public Type type { get => _type; init => _type                 = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Cast.cs:                 */ public partial record Cast                                                                { public SyntaxList<Hlsl, ArrayRank> arrayRankSpecifiers { get => _arrayRankSpecifiers; init => _arrayRankSpecifiers  = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Cast.cs:                 */ public partial record Cast                                                                { public CloseParenToken closeParenToken { get => _closeParenToken; init => _closeParenToken      = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Cast.cs:                 */ public partial record Cast                                                                { public Expression expression { get => _expression; init => _expression           = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Binary.cs:               */ public partial record Binary                                                              { public Expression left { get => _left; init => _left                 = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Binary.cs:               */ public partial record Binary                                                              { public Token<Hlsl> operatorToken { get => _operatorToken; init => _operatorToken        = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Binary.cs:               */ public partial record Binary                                                              { public Expression right { get => _right; init => _right                = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Call.cs:                 */ public partial record Call                                                                { public Identifier id { get => _id; init => _id                   = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Call.cs:                 */ public partial record Call                                                                { public ArgumentList<Syntax<Hlsl>> argList { get => _argList; init => _argList              = value with { Parent = this }; } }
//
// /*./Expressions/Operators/AssignmentExpression.cs: */ public partial record AssignmentExpression                                                { public Expression left { get => _left; init => _left                 = value with { Parent = this }; } }
//
// /*./Expressions/Operators/AssignmentExpression.cs: */ public partial record AssignmentExpression                                                { public AssignmentToken assignmentToken { get => _assignmentToken; init => _assignmentToken      = value with { Parent = this }; } }
//
// /*./Expressions/Operators/AssignmentExpression.cs: */ public partial record AssignmentExpression                                                { public Expression right { get => _right; init => _right                = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Member.cs:               */ public partial record Member                                                              { public Expression expression { get => _expression; init => _expression           = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Member.cs:               */ public partial record Member                                                              { public DotToken dotToken { get => _dotToken; init => _dotToken             = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Member.cs:               */ public partial record Member                                                              { public Identifier member { get => _member; init => _member               = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Affix.cs:                */ public partial record Affix                                                               { public Identifier id { get => _id; init => _id                   = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Affix.cs:                */ public partial record Affix                    { public partial record Pre                { public AffixOperatorToken prefixOperator { get => _prefixOperator; init => _prefixOperator       = value with { Parent = this }; } } }
//
// /*./Expressions/Operators/Affix.cs:                */ public partial record Affix                    { public partial record Post               { public AffixOperatorToken suffixOperator { get => _suffixOperator; init => _suffixOperator       = value with { Parent = this }; } } }
//
// /*./Expressions/Operators/Unary.cs:                */ public partial record Unary                                                               { public Token<Hlsl> operatorToken { get => _operatorToken; init => _operatorToken        = value with { Parent = this }; } }
//
// /*./Expressions/Operators/Unary.cs:                */ public partial record Unary                                                               { public Expression expression { get => _expression; init => _expression           = value with { Parent = this }; } }
//
// /*./Expressions/StructInitializer.cs:              */ public partial record StructInitializer                                                   { public BracedList<Expression> components { get => _components; init => _components           = value with { Parent = this }; } }
//
// /*./Expressions/Parenthesized.cs:                  */ public partial record Parenthesized                                                       { public OpenParenToken openParen { get => _openParen; init => _openParen            = value with { Parent = this }; } }
//
// /*./Expressions/Parenthesized.cs:                  */ public partial record Parenthesized                                                       { public Expression expression { get => _expression; init => _expression           = value with { Parent = this }; } }
//
// /*./Expressions/Parenthesized.cs:                  */ public partial record Parenthesized                                                       { public CloseParenToken closeParen { get => _closeParen; init => _closeParen           = value with { Parent = this }; } }
//
// /*./VariableDeclarator.cs:                         */ public partial record VariableDeclarator                                                  { public Token<Hlsl>? storageKeyword { get => _storageKeyword; init => _storageKeyword       = value with { Parent = this }; } }
//
// /*./VariableDeclarator.cs:                         */ public partial record VariableDeclarator                                                  { public Token<Hlsl>? typeModifier { get => _typeModifier; init => _typeModifier         = value with { Parent = this }; } }
//
// /*./VariableDeclarator.cs:                         */ public partial record VariableDeclarator                                                  { public Type type { get => _type; init => _type                 = value with { Parent = this }; } }
//
// /*./VariableDeclarator.cs:                         */ public partial record VariableDeclarator                                                  { public SeparatedList<Hlsl, VariableDefinition> variables { get => _variables; init => _variables            = value with { Parent = this }; } }
//
// /*./VariableDeclarator.cs:                         */ public partial record VariableDeclarator       { public partial record VariableDefinition { public Identifier id { get => _id; init => _id                   = value with { Parent = this }; } } }
//
// /*./VariableDeclarator.cs:                         */ public partial record VariableDeclarator       { public partial record VariableDefinition { public SyntaxList<Hlsl, ArrayRank>? arraySizes { get => _arraySizes; init => _arraySizes           = value with { Parent = this }; } } }
//
// /*./VariableDeclarator.cs:                         */ public partial record VariableDeclarator       { public partial record VariableDefinition { public Semantic? semantic { get => _semantic; init => _semantic             = value with { Parent = this }; } } }
//
// /*./VariableDeclarator.cs:                         */ public partial record VariableDeclarator       { public partial record VariableDefinition { public Initializer? initializer { get => _initializer; init => _initializer          = value with { Parent = this }; } } }
//
// /*./ArrayRank.cs:                                  */ public partial record ArrayRank                                                           { public OpenBracketToken openBracketToken { get => _openBracketToken; init => _openBracketToken     = value with { Parent = this }; } }
//
// /*./ArrayRank.cs:                                  */ public partial record ArrayRank                                                           { public LiteralExpression<IntLiteral> dimension { get => _dimension; init => _dimension            = value with { Parent = this }; } }
//
// /*./ArrayRank.cs:                                  */ public partial record ArrayRank                                                           { public CloseBracketToken closeBracketToken { get => _closeBracketToken; init => _closeBracketToken    = value with { Parent = this }; } }
//
//     public partial record Type                 { /*./Type.Struct.cs:                                */ public partial record Struct                                                              { public StructKeyword structKeyword { get => _structKeyword; init => _structKeyword        = value with { Parent = this }; } } }
//
//     public partial record Type                 { /*./Type.Struct.cs:                                */ public partial record Struct                                                              { public Identifier? name { get => _name; init => _name                 = value with { Parent = this }; } } }
//
//     public partial record Type                 { /*./Type.Struct.cs:                                */ public partial record Struct                                                              { public OpenBraceToken openBrace { get => _openBrace; init => _openBrace            = value with { Parent = this }; } } }
//
//     public partial record Type                 { /*./Type.Struct.cs:                                */ public partial record Struct                                                              { public SyntaxList<Hlsl, Member> members { get => _members; init => _members              = value with { Parent = this }; } } }
//
//     public partial record Type                 { /*./Type.Struct.cs:                                */ public partial record Struct                                                              { public CloseBraceToken closeBrace { get => _closeBrace; init => _closeBrace           = value with { Parent = this }; } } }
//
//     public partial record Type                 { /*./Type.Struct.cs:                                */ public partial record Struct                   { public partial record Member             { public Token<Hlsl>? interpolation { get => _interpolation; init => _interpolation        = value with { Parent = this }; } } } }
//
//     public partial record Type                 { /*./Type.Struct.cs:                                */ public partial record Struct                   { public partial record Member             { public Type type { get => _type; init => _type                 = value with { Parent = this }; } } } }
//
//     public partial record Type                 { /*./Type.Struct.cs:                                */ public partial record Struct                   { public partial record Member             { public Identifier id { get => _id; init => _id                   = value with { Parent = this }; } } } }
//
//     public partial record Type                 { /*./Type.Struct.cs:                                */ public partial record Struct                   { public partial record Member             { public Semantic? semantic { get => _semantic; init => _semantic             = value with { Parent = this }; } } } }
//
//     public partial record Type                 { /*./Type.Struct.cs:                                */ public partial record Struct                   { public partial record Member             { public SemiToken semicolon { get => _semicolon; init => _semicolon            = value with { Parent = this }; } } } }
//
//     public partial record Type                 { /*./Type.Predefined.cs:                            */ public partial record Predefined                                                          { public PredefinedTypeToken typeToken { get => _typeToken; init => _typeToken            = value with { Parent = this }; } } }
//
//     public partial record Type                 { /*./Type.UserDefined.cs:                           */ public partial record UserDefined                                                         { public Identifier id { get => _id; init => _id                   = value with { Parent = this }; } } }
//
// /*./Preprocessor/Line.cs:                          */ public partial record Line                                                                { public LineKeyword lineKeyword { get => _lineKeyword; init => _lineKeyword          = value with { Parent = this }; } }
//
// /*./Preprocessor/Line.cs:                          */ public partial record Line                                                                { public IntLiteral lineNumber { get => _lineNumber; init => _lineNumber           = value with { Parent = this }; } }
//
// /*./Preprocessor/Line.cs:                          */ public partial record Line                                                                { public QuotedStringLiteral? file { get => _file; init => _file                 = value with { Parent = this }; } }
//
// /*./Preprocessor/Ifndef.cs:                        */ public partial record Ifndef                                                              { public IfndefKeyword ifndefKeyword { get => _ifndefKeyword; init => _ifndefKeyword        = value with { Parent = this }; } }
//
// /*./Preprocessor/Ifndef.cs:                        */ public partial record Ifndef                                                              { public Identifier id { get => _id; init => _id                   = value with { Parent = this }; } }
//
// /*./Preprocessor/Include.cs:                       */ public partial record Include                                                             { public IncludePreprocessorKeyword includeKeyword { get => _includeKeyword; init => _includeKeyword       = value with { Parent = this }; } }
//
// /*./Preprocessor/Include.cs:                       */ public partial record Include                                                             { public QuotedStringLiteral filepath { get => _filepath; init => _filepath             = value with { Parent = this }; } }
//
// /*./Preprocessor/Undef.cs:                         */ public partial record Undef                                                               { public UndefKeyword undefKeyword { get => _undefKeyword; init => _undefKeyword         = value with { Parent = this }; } }
//
// /*./Preprocessor/Undef.cs:                         */ public partial record Undef                                                               { public Identifier id { get => _id; init => _id                   = value with { Parent = this }; } }
//
// /*./Preprocessor/Define.cs:                        */ public partial record Define                                                              { public DefineKeyword defineKeyword { get => _defineKeyword; init => _defineKeyword        = value with { Parent = this }; } }
//
// /*./Preprocessor/Define.cs:                        */ public partial record Define                                                              { public ArgumentList<Identifier>? argList { get => _argList; init => _argList              = value with { Parent = this }; } }
//
// /*./Preprocessor/Define.cs:                        */ public partial record Define                                                              { public Identifier id { get => _id; init => _id                   = value with { Parent = this }; } }
//
// /*./Preprocessor/Define.cs:                        */ public partial record Define                                                              { public TokenString tokenString { get => _tokenString; init => _tokenString          = value with { Parent = this }; } }
//
// /*./Preprocessor/If.cs:                            */ public partial record If                                                                  { public IfKeyword ifKeyword { get => _ifKeyword; init => _ifKeyword            = value with { Parent = this }; } }
//
// /*./Preprocessor/If.cs:                            */ public partial record If                                                                  { public TokenString condition { get => _condition; init => _condition            = value with { Parent = this }; } }
//
// /*./Preprocessor/Else.cs:                          */ public partial record Else                                                                { public ElseKeyword elseKeyword { get => _elseKeyword; init => _elseKeyword          = value with { Parent = this }; } }
//
// /*./Preprocessor/Elif.cs:                          */ public partial record Elif                                                                { public ElifKeyword elifKeyword { get => _elifKeyword; init => _elifKeyword          = value with { Parent = this }; } }
//
// /*./Preprocessor/Elif.cs:                          */ public partial record Elif                                                                { public TokenString condition { get => _condition; init => _condition            = value with { Parent = this }; } }
//
// /*./Preprocessor/Error.cs:                         */ public partial record Error                                                               { public ErrorKeyword errorKeyword { get => _errorKeyword; init => _errorKeyword         = value with { Parent = this }; } }
//
// /*./Preprocessor/Error.cs:                         */ public partial record Error                                                               { public TokenString tokenstring { get => _tokenstring; init => _tokenstring          = value with { Parent = this }; } }
//
// /*./Preprocessor/Ifdef.cs:                         */ public partial record Ifdef                                                               { public IfdefKeyword ifdefKeyword { get => _ifdefKeyword; init => _ifdefKeyword         = value with { Parent = this }; } }
//
// /*./Preprocessor/Ifdef.cs:                         */ public partial record Ifdef                                                               { public Identifier id { get => _id; init => _id                   = value with { Parent = this }; } }
//
// /*./Preprocessor/Endif.cs:                         */ public partial record Endif                                                               { public EndIfKeyword endifKeyword { get => _endifKeyword; init => _endifKeyword         = value with { Parent = this }; } }
//
// /*./Preprocessor/Pragma.cs:                        */ public partial record Pragma                                                              { public PragmaKeyword pragmaKeyword { get => _pragmaKeyword; init => _pragmaKeyword        = value with { Parent = this }; } }
//
// /*./Preprocessor/Pragma.cs:                        */ public partial record Pragma                                                              { public TokenString? tokenString { get => _tokenString; init => _tokenString          = value with { Parent = this }; } }
//
// /*./Preprocessor/PreprocessorSyntax.cs:            */ public partial record PreprocessorSyntax                                                  { public HashToken hashToken { get => _hashToken; init => _hashToken            = value with { Parent = this }; } }
//
// /*./Trivias/PreprocessorDirectives.cs:             */ public abstract partial record PreprocessorDirective                                      { public PreprocessorSyntax triviaSyntax { get => _triviaSyntax; init => _triviaSyntax         = value with { Parent = null }; } }
// }