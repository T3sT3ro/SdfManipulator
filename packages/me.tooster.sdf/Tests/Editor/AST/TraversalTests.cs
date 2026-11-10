using System.Collections.Generic;
using System.Linq;
using System.Text;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using NUnit.Framework;
using static me.tooster.sdf.AST.Hlsl.Extensions;
using HlslMapper = me.tooster.sdf.AST.Hlsl.Mapper;
using ShaderlabMapper = me.tooster.sdf.AST.Shaderlab.Mapper;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;

namespace me.tooster.sdf.Tests.Editor.AST {
    /// <summary>
    ///     Walkers and mappers visit every token of the printing test cases in syntax order, and give every node the anchor of
    ///     its parent.
    /// </summary>
    public class TraversalTests {
        static IEnumerable<TestCaseData> HlslTrees      => HlslPrintingTests.Trees.Select(c => new TestCaseData(c.tree).SetName($"{{m}} {c.name}"));
        static IEnumerable<TestCaseData> ShaderlabTrees => ShaderlabPrintingTests.Trees.Select(c => new TestCaseData(c.tree).SetName($"{{m}} {c.name}"));

        [TestCaseSource(nameof(HlslTrees))]
        public static void WalkerVisitsEveryHlslToken(Tree<hlsl>.Node tree) => AssertWalkerVisitsEveryToken(tree);

        [TestCaseSource(nameof(ShaderlabTrees))]
        public static void WalkerVisitsEveryShaderlabToken(Tree<shaderlab>.Node tree) => AssertWalkerVisitsEveryToken(tree);

        [TestCaseSource(nameof(HlslTrees))]
        public static void MapperVisitsHlslTokensInSyntaxOrder(Tree<hlsl>.Node tree) {
            var mapper = new HlslTokenRecorder();
            var mapped = mapper.Map(tree);

            Assert.AreSame(tree, mapped, "a mapper that changes nothing returns the same tree");
            CollectionAssert.AreEqual(TokensInSyntaxOrder(tree), mapper.tokens);
        }

        [TestCaseSource(nameof(ShaderlabTrees))]
        public static void MapperVisitsShaderlabTokensInSyntaxOrder(Tree<shaderlab>.Node tree) {
            var mapper = new ShaderlabTokenRecorder();
            var mapped = mapper.Map(tree);

            Assert.AreSame(tree, mapped, "a mapper that changes nothing returns the same tree");
            // the mapper leaves injected HLSL as it is, including the tokens that mark its start and end
            var expected = TokensInSyntaxOrder(tree)
                .Where(t => t is not InjectedLanguageStartToken<shaderlab> and not InjectedLanguageEndToken<shaderlab>);
            CollectionAssert.AreEqual(expected, mapper.tokens);
        }

        [Test]
        public static void WalkerDescendsIntoDirectivesOnlyWhenAsked() {
            var statement = Var(((Type)new FloatKeyword()).WithLeadingTrivia<Type, hlsl>(new Pragma { tokenString = "target 5.0" }.ToStructuredTrivia()), "x");

            Assert.That(TokenTexts(statement, false), Is.EqualTo(new[] { "float", "x", ";" }));
            Assert.That(TokenTexts(statement, true), Is.EqualTo(new[] { "#", "pragma", "target 5.0", "", "float", "x", ";" }));
        }

        [Test]
        public static void MapperRebuildsOnlyThePathToAChangedNode() {
            var function = new FunctionDefinition
            {
                returnType = (Type)new FloatKeyword(),
                id = "f",
                paramList = new Parameter { type = (Type)new FloatKeyword(), id = "x" },
                body = new Statement[] { new Return { expression = (Identifier)"x" } },
            };

            var renamed = (FunctionDefinition)new Renamer("x", "y").Map(function)!;

            Assert.AreEqual("floatf(floaty){returny;}", renamed.ToString());
            Assert.AreEqual("floatf(floatx){returnx;}", function.ToString(), "the original tree doesn't change");
            Assert.AreSame(function.returnType, renamed.returnType, "unchanged subtrees are shared");
            Assert.AreSame(function.id, renamed.id);
            Assert.AreNotSame(function.body, renamed.body);
        }

        /// The walker visits every token, so their trivia and text make up the text of the tree.
        static void AssertWalkerVisitsEveryToken<Lang>(Tree<Lang>.Node tree) {
            var walker = new TextWalker<Lang>();
            walker.Walk(tree);

            Assert.AreEqual(tree.ToString(), walker.text.ToString());
        }

        static List<Token<Lang>> TokensInSyntaxOrder<Lang>(Tree<Lang>.Node tree) {
            var walker = new TokenWalker<Lang>(false);
            walker.Walk(tree);
            return walker.tokens;
        }

        static IEnumerable<string> TokenTexts<Lang>(Tree<Lang>.Node tree, bool descendIntoStructuredTrivia) {
            var walker = new TokenWalker<Lang>(descendIntoStructuredTrivia);
            walker.Walk(tree);
            return walker.tokens.Select(t => t.Text);
        }

        /// Every anchor up to the root points at the parent of its node: a syntax node, a token for its trivia list, and so on.
        static void AssertAnchoredToParent<Lang>(IAnchor anchor) {
            for (var a = anchor; a.Parent is not null; a = a.Parent) {
                Assert.AreNotSame(a.Node, a.Parent.Node, $"{a.Node.GetType().Name} is anchored to itself");
                if (a is { Node: SyntaxOrToken<Lang> node, Parent: { Node: Syntax<Lang> parent } })
                    Assert.That(parent.ChildNodesAndTokens().Any(c => ReferenceEquals(c, node)), $"{node.GetType().Name} is anchored to {parent.GetType().Name}, which isn't its parent");
            }
        }



        class TextWalker<Lang> : Walker<Lang> {
            public readonly StringBuilder text = new();

            public override void Visit(Anchor<Token<Lang>> a) {
                AssertAnchoredToParent<Lang>(a);
                text.Append(a.Node.LeadingTriviaList).Append(a.Node.Text);
            }

            public override void Visit(Anchor<Syntax<Lang>> a) {
                AssertAnchoredToParent<Lang>(a);
                base.Visit(a);
            }

            public override void Visit(Anchor<Statement<Lang>> a) {
                AssertAnchoredToParent<Lang>(a);
                base.Visit(a);
            }

            public override void Visit(Anchor<Expression<Lang>> a) {
                AssertAnchoredToParent<Lang>(a);
                base.Visit(a);
            }

            // the injected tree is in another language, so a walker of this language doesn't visit it
            public override void Visit<T>(Anchor<InjectedLanguage<Lang, T>> a) => text.Append(a.Node);
        }



        class TokenWalker<Lang> : Walker<Lang> {
            public readonly List<Token<Lang>> tokens = new();

            public TokenWalker(bool descendIntoStructuredTrivia) : base(descendIntoStructuredTrivia) { }

            public override void Visit(Anchor<Token<Lang>> a) {
                base.Visit(a); // trivia, including the directives before the token, comes first
                tokens.Add(a.Node);
            }
        }



        class HlslTokenRecorder : HlslMapper {
            public readonly List<Token<hlsl>> tokens = new();

            public override Tree<hlsl>.Node? Visit(Anchor<Token<hlsl>> a) {
                AssertAnchoredToParent<hlsl>(a);
                tokens.Add(a.Node);
                return base.Visit(a);
            }
        }



        class ShaderlabTokenRecorder : ShaderlabMapper {
            public readonly List<Token<shaderlab>> tokens = new();

            public override Tree<shaderlab>.Node? Visit(Anchor<Token<shaderlab>> a) {
                AssertAnchoredToParent<shaderlab>(a);
                tokens.Add(a.Node);
                return base.Visit(a);
            }
        }



        class Renamer : HlslMapper {
            readonly string from, to;

            public Renamer(string from, string to) {
                this.from = from;
                this.to = to;
            }

            public override Tree<hlsl>.Node? Visit(Anchor<Token<hlsl>> a)
                => a.Node is IdentifierToken token && token.Text == from ? token with { ValidatedText = to } : base.Visit(a);
        }
    }
}
