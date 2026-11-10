using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using NUnit.Framework;
using static me.tooster.sdf.AST.Hlsl.Extensions;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using If = me.tooster.sdf.AST.Hlsl.Syntax.Statements.If;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;

namespace me.tooster.sdf.Tests.Editor.AST {
    /// <summary>
    ///     Every HLSL syntax node prints valid HLSL after formatting. <see cref="EveryNodeTypeHasAPrintingTest" /> checks that
    ///     the cases cover every node type.
    /// </summary>
    public class HlslPrintingTests {
        static Type Float  => new FloatKeyword();
        static Type Float3 => new VectorTypeToken { arity = 3 };
        static Type Float4 => new VectorTypeToken { arity = 4 };

        static Identifier        Id(string name)  => name;
        static LiteralExpression Literal(float v) => v;
        static LiteralExpression Literal(int v)   => v;
        static Statement         Statement(Expression e) => (ExpressionStatement)e;

        static Binary Binary(Expression left, Token<hlsl> op, Expression right) => new() { left = left, operatorToken = op, right = right };
        static Unary  Negate(Expression e) => new() { operatorToken = new MinusToken(), expression = e };

        static AssignmentExpression Assign(string left, Expression right) => new() { left = Id(left), right = right };

        static Block Body(params Statement[] statements) => statements;

        static VariableDefinition Definition(VariableDeclarator declarator) => declarator;

        /// <c>float x;</c> with <paramref name="directive" /> on the line before it
        static Statement After(PreprocessorSyntax directive)
            => Definition(new VariableDeclarator
            {
                type = Float.WithLeadingTrivia<Type, hlsl>(directive.ToStructuredTrivia()),
                variables = new VariableDeclarator.Definition { id = "x" },
            });

        static IEnumerable<(string name, Tree<hlsl>.Node node, string expected)> Cases() {
            // expressions
            yield return ("Identifier", Statement(Id("foo")), "foo;");
            yield return ("LiteralExpression", Var(Float, "x", Literal(0.5f)), "float x = 0.5;");
            yield return ("BooleanLiteral", Var(new BoolKeyword(), "b", (LiteralExpression)true), "bool b = true;");
            yield return ("Parenthesized", Statement(Binary(new Parenthesized { expression = Binary(Id("a"), new PlusToken(), Id("b")) }, new AsteriskToken(), Id("c"))),
                "(a + b) * c;");
            yield return ("Binary", Statement(Binary(Binary(Id("a"), new AsteriskToken(), Id("b")), new PlusToken(), Id("c"))), "a * b + c;");
            yield return ("Binary with a negative literal", Statement(Binary(Id("a"), new MinusToken(), Literal(-0.5f))), "a - -0.5;");
            yield return ("Unary", Statement(Negate(Id("x"))), "-x;");
            yield return ("Unary not", Statement(new Unary { operatorToken = new NotToken(), expression = Id("c") }), "!c;");
            yield return ("Unary of a negative literal", Statement(Negate(Literal(-0.5f))), "- -0.5;");
            yield return ("Unary of a unary", Statement(Negate(Negate(Id("x")))), "- -x;");
            yield return ("Affix.Pre", Statement(new Affix.Pre { prefixOperator = new PlusPlusToken(), id = "i" }), "++i;");
            yield return ("Affix.Post", Statement(new Affix.Post { postfixOperator = new MinusMinusToken(), id = "i" }), "i--;");
            yield return ("AssignmentExpression", Statement(new AssignmentExpression { left = Id("x"), assignmentToken = new PlusEqualsToken(), right = Literal(1) }),
                "x += 1;");
            yield return ("Call", Statement(FunctionCall("max", Id("a"), Literal(0f))), "max(a, 0.0);");
            yield return ("QualifiedNameSyntax", Statement(FunctionCall("sdf::operators::smoothUnion", Id("a"), Id("b"))), "sdf::operators::smoothUnion(a, b);");
            yield return ("Cast", Statement(new Cast { type = Float3, expression = Literal(0) }), "(float3) 0;");
            yield return ("Comma", Statement(new Comma { left = Assign("a", Literal(1)), right = Assign("b", Literal(2)) }), "a = 1, b = 2;");
            yield return ("Indexer", Statement(new Indexer { expression = Id("a"), index = Literal(0) }), "a[0];");
            yield return ("Member", Statement(new Member { expression = Id("p"), member = "xyz" }), "p.xyz;");
            yield return ("Ternary", Statement(new Ternary { condition = Id("c"), whenTrue = Id("a"), whenFalse = Id("b") }), "c ? a : b;");
            yield return ("BracedInitializerExpression", Var(new VectorTypeToken { arity = 2 }, "v", (BracedInitializerExpression)new Expression[] { Literal(1f), Literal(2f) }),
                "float2 v = { 1.0, 2.0 };");

            // declarations
            yield return ("ArrayRank", Definition(new VariableDeclarator
                {
                    type = Float,
                    variables = new VariableDeclarator.Definition { id = "a", arraySizes = new ArrayRank { dimension = Literal(4) } },
                }),
                "float a[4];");
            yield return ("VariableDeclarator with several variables", Definition(new VariableDeclarator
                {
                    storageKeywords = new SyntaxOrTokenList<hlsl>(new UniformKeyword()),
                    type = new MatrixTypeToken { rows = 3, cols = 3 },
                    variables = new[]
                    {
                        new VariableDeclarator.Definition { id = "a" },
                        new VariableDeclarator.Definition { id = "b", semantic = (Semantic)new TexcoordSemantic { n = 1 } },
                    }.CommaSeparated(),
                }),
                "uniform float3x3 a, b : TEXCOORD1;");
            yield return ("Type.UserDefined", Var("Result", "r"), "Result r;");
            yield return ("Typedef", new Typedef { type = Float3, id = "position" }, "typedef float3 position;");
            yield return ("StructDefinition", (StructDefinition)new Type.Struct
                {
                    name = "Result",
                    members = new[]
                    {
                        new Type.Struct.Member { type = Float, id = "distance" },
                        new Type.Struct.Member { interpolation = new NointerpolationKeyword(), type = Float3, id = "normal", semantic = (Semantic)new NormalSemantic() },
                    },
                },
                "struct Result {\n    float distance;\n    nointerpolation float3 normal : NORMAL;\n};");
            yield return ("FunctionDeclaration", new FunctionDeclaration
                {
                    returnType = Float4,
                    id = "frag",
                    paramList = new[]
                    {
                        new Parameter { modifier = new InKeyword(), type = Float4, id = "position", semantic = (Semantic)new SvPositionSemantic() },
                        new Parameter { type = Float, id = "scale", initializer = Literal(1f) },
                    },
                    returnSemantic = (Semantic)new SvTargetSemantic { n = 0 },
                },
                "float4 frag(in float4 position : SV_Position, float scale = 1.0) : SV_Target0;");
            yield return ("FunctionDefinition", new FunctionDefinition
                {
                    returnType = Float,
                    id = "f",
                    paramList = new Parameter { type = Float, id = "x" },
                    body = Body(new Return { expression = Id("x") }),
                },
                "float f(float x) {\n    return x;\n}");

            // statements
            yield return ("ExpressionStatement without an expression", new ExpressionStatement(), ";");
            yield return ("If", new If
                {
                    test = Id("c"),
                    then = Body(new Discard()),
                    elseClause = new If.ElseClause
                    {
                        statement = new If { test = Id("d"), then = Body(Statement(Assign("x", Literal(1)))), elseClause = new If.ElseClause { statement = Body(new Break()) } },
                    },
                },
                "if (c) {\n    discard;\n} else if (d) {\n    x = 1;\n} else {\n    break;\n}");
            yield return ("While", new While { test = Binary(Id("i"), new LessThanToken(), Literal(4)), body = Body(new Continue()) },
                "while (i < 4) {\n    continue;\n}");
            yield return ("DoWhile", new DoWhile { body = Statement(new Affix.Post { postfixOperator = new PlusPlusToken(), id = "i" }), test = Binary(Id("i"), new LessThanToken(), Literal(4)) },
                "do {\n    i++;\n} while (i < 4);");
            yield return ("For", new For
                {
                    initializer = new VariableDeclarator { type = new IntKeyword(), variables = new VariableDeclarator.Definition { id = "i", initializer = (Expression)Literal(0) } },
                    condition = Binary(Id("i"), new LessThanToken(), Literal(4)),
                    increments = ((Expression)new Affix.Post { postfixOperator = new PlusPlusToken(), id = "i" }).CommaSeparated(),
                    body = Body(new Break()),
                },
                "for (int i = 0; i < 4; i++) {\n    break;\n}");
            yield return ("For.VariableInitializer", new For
                {
                    initializer = new For.VariableInitializer { initializers = new[] { Assign("i", Literal(0)), Assign("j", Literal(1)) }.CommaSeparated() },
                    condition = Binary(Id("i"), new LessThanToken(), Id("j")),
                    increments = new Expression[] { new Affix.Post { postfixOperator = new PlusPlusToken(), id = "i" }, new Affix.Post { postfixOperator = new MinusMinusToken(), id = "j" } }.CommaSeparated(),
                    body = Body(),
                },
                "for (i = 0, j = 1; i < j; i++, j--) {\n}");
            yield return ("Switch", new Switch
                {
                    selector = Id("mode"),
                    cases = new Switch.Case { label = 0, body = new[] { Statement(Assign("x", Literal(1))), new Break() } },
                    defaultCase = new Switch.DefaultCase { body = new[] { Statement(Assign("x", Literal(0))) } },
                },
                "switch (mode) {\n    case 0:\n        x = 1;\n        break;\n    default:\n        x = 0;\n}");

            // preprocessor directives are structured trivia of the token that follows them
            yield return ("Define", After(new Define { id = "FOO", argList = (ArgumentList<Identifier>)Id("x"), tokenString = "((x) * 2)" }),
                "#define FOO(x) ((x) * 2)\nfloat x;");
            yield return ("Undef", After(new Undef { id = "FOO" }), "#undef FOO\nfloat x;");
            yield return ("If directive", After(new me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor.If { condition = "defined(FOO)" }), "#if defined(FOO)\nfloat x;");
            yield return ("Ifdef", After(new Ifdef { id = "FOO" }), "#ifdef FOO\nfloat x;");
            yield return ("Ifndef", After(new Ifndef { id = "FOO" }), "#ifndef FOO\nfloat x;");
            yield return ("Elif", After(new Elif { condition = "FOO > 1" }), "#elif FOO > 1\nfloat x;");
            yield return ("Else", After(new Else()), "#else\nfloat x;");
            yield return ("Endif", After(new Endif()), "#endif\nfloat x;");
            yield return ("Include", After(new Include { filepath = "UnityCG.cginc" }), "#include \"UnityCG.cginc\"\nfloat x;");
            yield return ("Include with pragmas", After(new Include { includeKeyword = new IncludeWithPragmasKeyword(), filepath = "a.hlsl" }),
                "#include_with_pragmas \"a.hlsl\"\nfloat x;");
            yield return ("Line", After(new Line { lineNumber = 10, file = "a.hlsl" }), "#line 10 \"a.hlsl\"\nfloat x;");
            yield return ("Pragma", After(new Pragma { tokenString = "target 5.0" }), "#pragma target 5.0\nfloat x;");
            yield return ("Error", After(new Error { tokenstring = "unsupported" }), "#error unsupported\nfloat x;");
        }

        static IEnumerable<TestCaseData> PrintingCases() => Cases().Select(c => new TestCaseData(c.node, c.expected).SetName($"Prints {c.name}"));

        /// All printing cases, for tests of the visitors.
        internal static IEnumerable<(string name, Tree<hlsl>.Node tree)> Trees => Cases().Select(c => (c.name, c.node));

        [TestCaseSource(nameof(PrintingCases))]
        public static void Prints(Tree<hlsl>.Node node, string expected) => Assert.AreEqual(expected, HlslFormatter.Format(node)!.ToString());

        [Test]
        public static void EveryNodeTypeHasAPrintingTest() {
            var printed = Trees.SelectMany(c => SyntaxTypes.Of(c.tree)).ToHashSet();

            var missing = SyntaxTypes.All<hlsl>("me.tooster.sdf.AST.Hlsl").Where(t => !printed.Contains(t)).Select(t => t.FullName).ToList();

            Assert.IsEmpty(missing, "Add a printing test case for these node types.");
        }

        [Test]
        public static void FunctionsAreSeparatedByABlankLine() {
            FunctionDefinition function(string name) => new() { returnType = Float, id = name, body = Body(new Return { expression = Literal(1f) }) };
            var statements = new SyntaxList<hlsl, Statement>(
                new FunctionDeclaration { returnType = Float, id = "f" },
                function("f"),
                function("g"),
                Var(Float, "x")
            );

            Assert.AreEqual(
                "float f();\n\nfloat f() {\n    return 1.0;\n}\n\nfloat g() {\n    return 1.0;\n}\nfloat x;",
                HlslFormatter.Format(statements)!.ToString()
            );
        }

        [Test]
        public static void DirectiveInTheMiddleOfTheTreeStaysOnItsOwnLine() {
            var block = Body(
                Statement(Assign("a", Literal(1))),
                Statement(Assign("b", Literal(2))).WithLeadingTrivia<Statement, hlsl>(new Pragma { tokenString = "multi_compile _ FOO" }.ToStructuredTrivia())
            );

            Assert.AreEqual("{\n    a = 1;\n    #pragma multi_compile _ FOO\n    b = 2;\n}", HlslFormatter.Format(block)!.ToString());
        }

        [Test]
        public static void BracketsDontChangeTheIndentationOfWhatFollows() {
            var block = Body(Statement(new Indexer { expression = Id("a"), index = Literal(0) }), Statement(Id("b")));

            Assert.AreEqual("{\n    a[0];\n    b;\n}", HlslFormatter.Format(block)!.ToString());
        }

        [Test]
        public static void NestedBracedListsIndentByOneLevel() {
            var row1 = ((BracedInitializerExpression)new Expression[] { Literal(1f), Literal(0f) }).WithLeadingTrivia<BracedInitializerExpression, hlsl>(new me.tooster.sdf.AST.Syntax.CommonTrivia.NewLine<hlsl>());
            var row2 = ((BracedInitializerExpression)new Expression[] { Literal(0f), Literal(1f) }).WithLeadingTrivia<BracedInitializerExpression, hlsl>(new me.tooster.sdf.AST.Syntax.CommonTrivia.NewLine<hlsl>());
            var matrix = new BracedInitializerExpression
            {
                components = new BracedList<Expression>
                {
                    arguments = new Expression[] { row1, row2 }.CommaSeparated(),
                    closeBraceToken = new CloseBraceToken { LeadingTriviaList = new me.tooster.sdf.AST.Syntax.CommonTrivia.NewLine<hlsl>() },
                },
            };

            Assert.AreEqual(
                "float2x2 m = {\n    { 1.0, 0.0 },\n    { 0.0, 1.0 }\n};",
                HlslFormatter.Format(Var(new MatrixTypeToken { rows = 2, cols = 2 }, "m", matrix))!.ToString()
            );
        }

        [TestCase(1f, "1.0")]
        [TestCase(0.5f, "0.5")]
        [TestCase(-0.5f, "-0.5")]
        [TestCase(0.1f, "0.1")]
        [TestCase(0.6016249f, "0.6016249")]
        [TestCase(123456.7f, "123456.7")]
        [TestCase(1e-7f, "0.0000001")]
        [TestCase(-1.5e-7f, "-0.00000015")]
        [TestCase(3e10f, "30000000000.0")]
        public static void FloatLiteralPrintsTheShortestDecimalThatReadsBackTheSame(float value, string expected) {
            var text = ((FloatLiteral)value).Text;

            Assert.AreEqual(expected, text);
            Assert.AreEqual(value, float.Parse(text, System.Globalization.CultureInfo.InvariantCulture));
        }

        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public static void FloatLiteralRejectsValuesWithoutALiteral(float value)
            => Assert.Throws<ArgumentOutOfRangeException>(() => _ = (FloatLiteral)value);

        [Test]
        public static void SemanticsPrintTheirHlslName() {
            var semanticTypes = typeof(SemanticToken).Assembly.GetTypes()
                .Where(t => typeof(SemanticToken).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var type in semanticTypes) {
                var text = ((SemanticToken)Activator.CreateInstance(type)).Text;
                StringAssert.DoesNotContain("Semantic", text, type.Name);
                StringAssert.IsMatch("^[A-Z][A-Za-z_]*$", text, type.Name);
            }

            Assert.AreEqual("SV_Target1", new SvTargetSemantic { n = 1 }.Text);
            Assert.AreEqual("SV_Position", new SvPositionSemantic().Text);
        }
    }
}
