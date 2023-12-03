#nullable enable
using System;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations;

namespace me.tooster.sdf.AST {
    /// <summary>
    ///    Navigator is a visitor that remembers the path to the current node
    /// </summary>
    /// <typeparam name="Lang"></typeparam>
    [Obsolete("This is a prototype research only implementation of something that goes over tree and keeps track anchors")]
    public partial class Navigator<Lang> : Visitor<Lang> {
        protected virtual void Visit(Anchor<FunctionDeclaration> a) {
            Visit((dynamic)new Anchor<Identifier>(a.Node.id, a));
            Visit((dynamic)new Anchor<Block>(a.Node.body, a));
        }
    }
    
    public class Rewriter<Lang> : Mapper<Lang> {
        protected virtual void Visit(Anchor<FunctionDeclaration> a) {
            Visit((dynamic)new Anchor<Identifier>(a.Node.id, a));
            Visit((dynamic)new Anchor<Block>(a.Node.body, a));
        }

        protected virtual void Visit(Anchor<Identifier> a) {
            Visit((dynamic)new Anchor<IdentifierToken>(a.Node.id, a));
        }

        public Rewriter(bool descentIntoTrivia) : base(descentIntoTrivia) { }
    }
}
