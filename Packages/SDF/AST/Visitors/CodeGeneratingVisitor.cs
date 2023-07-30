using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using AST.Hlsl;
using AST.Hlsl.Syntax;
using AST.Hlsl.Syntax.Expressions;
using AST.Hlsl.Syntax.Expressions.Literals;
using AST.Hlsl.Syntax.Expressions.Operators;
using AST.Hlsl.Syntax.Statements.Declarations;
using AST.Syntax;

namespace AST.Visitors {
    public class CodeGeneratingVisitor<Ttree, TNode, TBase>
        where Ttree : SyntaxTree<TNode, TBase>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {

        private StringBuilder sb;

        public string Generate(Ttree tree) {
            sb = new StringBuilder();
            Visit((dynamic)tree.Root);
            return sb.ToString();
        }


        public void Visit(HlslToken token) {
            foreach (var trivia in token.LeadingTrivia) {
                sb.Append(trivia.ToString());
                    sb.Append(token.Text);
            }
        }

        public void Visit(HlslSyntax syntax) {
            foreach (var node in syntax.ChildNodesAndTokens)
                Visit((dynamic)node);
        }
        
        public string Visit(IdentifierName id) =>
            id.idToken;

        // expressions

        public string Visit(Literal node) =>
            node.text;

        public string Visit(Call node) =>
            $"{node.id.name}({string.Join(", ", node.arguments.Select(arg => Visit((dynamic)arg)))})";

        public string Visit(Unary node) =>
            $"{Stringifier.Get(node.kind)}{Visit((dynamic)node.expression.ParenthesizeFor(node))}";

        public string Visit(Binary node) {
            var left = Visit((dynamic)node.left.ParenthesizeFor(node));
            var op = Stringifier.Get(node.kind);
            var right = Visit((dynamic)node.right.ParenthesizeFor(node));

            return $"{left} {op} {right}";
        }

        public string Visit(Ternary node) {
            var test = Visit((dynamic)node.condition.ParenthesizeFor(node));
            var ifTrue = Visit((dynamic)node.whenTrue.ParenthesizeFor(node));
            var ifFalse = Visit((dynamic)node.whenFalse.ParenthesizeFor(node));

            return $"{test} ? {ifTrue} : {ifFalse}";
        }

        public string Visit(Indexer node) =>
            $"{node.expression.ParenthesizeFor(node)}[{Visit((dynamic)node.index)}]";

        public string Visit(AssignmentExpresion node) =>
            $"{Visit((dynamic)node.left)} {Stringifier.Get(node.kind)} {Visit((dynamic)node.right)}";

        public string Visit(Member node) =>
            $"{Visit((dynamic)node.expression.ParenthesizeFor(node))}.{node.member.name}";

        public string Visit(Cast node) =>
            $"({Stringifier.Get(node.type)}){node.expression.ParenthesizeFor(node)}";

        public string Visit(Affix.Pre node) =>
            $"{Stringifier.Get(node.kind)}{Visit((dynamic)node.id)}";

        public string Visit(Affix.Post node) =>
            $"{Visit((dynamic)node.id)}{Stringifier.Get(node.kind)}";

        public string Visit(Comma node) =>
            $"{Visit((dynamic)node.left)}, {Visit((dynamic)node.right)}";

        // intermediate node

        public string Visit(Parenthesized node) =>
            $"({Visit((dynamic)node.expression)})";

        // statements

        public string Visit(Block node) {
            if (node.statements.Count <= 1)
                return Visit((dynamic)node.statements[0]);

            return $"{{\n{string.Join("\n", node.statements.Select(stmt => Visit((dynamic)stmt)))}\n}}";
        }

        public string Visit(Semantic node) =>
            $"{Stringifier.Get(node.kind)}{node.index?.ToString() ?? ""}";

        public string Visit(VariableDeclaration node) {
            var storage = node.storage != null ? Stringifier.Get(node.storage.GetValueOrDefault()) : null;
            var typeModifier = node.typeModifier != null
                ? Stringifier.Get(node.typeModifier.GetValueOrDefault())
                : null;
            var typeName = Visit((dynamic)node.type);
            var id =
                $"{Visit((dynamic)node.id)}{string.Join("", node.arraySizes?.Select(s => $"[{s}]") ?? Array.Empty<string>())}";
            var semantic = node.semantic != null ? $": {Visit((dynamic)node.semantic)}" : null;
            var init = node.initializer != null ? $"= {Visit((dynamic)node.initializer)}" : "";

            return
                @$"{string.Join(" ", new[] { storage, typeModifier, typeName, id, semantic, init }.Where(s => s != null))};";
        }

        public string Visit(FunctionDeclaration.Argument node) {
            var inputModifier = node.modifier != null ? Stringifier.Get(node.modifier.GetValueOrDefault()) : null;
            var typeName = Visit((dynamic)node.type);
            var id = Visit((dynamic)node.id);
            var semantic = node.semantic != null ? $": {Visit((dynamic)node.semantic)}" : null;
            var init = node.initializer != null ? $"= {Visit((dynamic)node.initializer)}" : "";

            return
                $"{string.Join(" ", new[] { inputModifier, typeName, id, semantic, init }.Where(s => s != null))}";
        }

        public string Visit(FunctionDeclaration node) {
            var returnType = Visit((dynamic)node.returnType);
            var id = Visit((dynamic)node.id);
            var parameters = string.Join(", ", node.arguments.Select(p => Visit((dynamic)p)));
            var semantic = node.returnSemantic != null ? $": {Visit((dynamic)node.returnSemantic)}" : null;
            var body = node.body != null ? Visit((dynamic)node.body) : ";";

            return $"{returnType} {id}({parameters}) {semantic} {body}";
        }

        public string Visit(StructDeclaration.Member node) {
            var interpolation = node.interpolation != null
                ? Stringifier.Get(node.interpolation.GetValueOrDefault())
                : null;
            var type = Visit((dynamic)node.type);
            var id = Visit((dynamic)node.id);
            var semantic = node.semantic != null ? $": {Visit((dynamic)node.semantic)}" : null;

            return $"{string.Join(" ", new[] { interpolation, type, id, semantic }.Where(s => s != null))};";
        }

        public string Visit(StructDeclaration node) {
            var type = Visit((dynamic)node.type);
            var members = string.Join("\n", node.members.Select(m => Visit((dynamic)m)));

            return $"struct {type} {{\n{members}\n}};";
        }

        public string Visit(If node) {
            var test = Visit((dynamic)node.test);
            var ifTrue = Visit((dynamic)node.then);
            var @else = node.@else != null ? $"else {Visit((dynamic)node.@else)}" : "";

            return $"if ({test}) {ifTrue} {@else}";
        }
    }
}
