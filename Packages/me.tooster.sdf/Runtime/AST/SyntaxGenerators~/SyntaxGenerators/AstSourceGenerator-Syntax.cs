using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

public partial class AstSourceGenerator {
    private void GenerateSyntaxClasses(ITypeSymbol recordSymbol) {
        var fileName = string.Join(".", getQualifiedNameParts(recordSymbol).Reverse()) + ".g.cs";


        GenerateInternalSyntax(fileName, recordSymbol);
        // GeneratePublicSyntax(fileName.Replace("InternalSyntax", "Syntax"), recordSymbol);
    }

    private void GenerateInternalSyntax(string name, ITypeSymbol recordSymbol) {
        var resultCompilation = CompilationUnit().WithMembers(SingletonList<MemberDeclarationSyntax>(
                NamespaceDeclaration(IdentifierName(recordSymbol.ContainingNamespace.ToString()))
                    .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                    .AddMembers(wrapRecordInEnclosingClasses(
                        generateInternalRecord(recordSymbol), recordSymbol))
                    .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken))))
            .AddUsings(
                UsingDirective(IdentifierName("System.Collections.Generic")),
                UsingDirective(IdentifierName("me.tooster.sdf.AST.Syntax"))
                // UsingDirective(IdentifierName($"me.tooster.sdf.AST.{getQualifiedNameParts(recordSymbol).Last()}"))
            )
            .NormalizeWhitespace();

        context.AddSource(name, SourceText.From(resultCompilation.SyntaxTree.ToString(), Encoding.UTF8));
    }

    private void GeneratePublicSyntax(string name, ITypeSymbol recordSymbol) {
        var resultCompilation = CompilationUnit().WithMembers(SingletonList<MemberDeclarationSyntax>(
                NamespaceDeclaration(
                        IdentifierName(recordSymbol.ContainingNamespace.Name.Replace("InternalSyntax", "Syntax")))
                    .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                    .AddMembers(wrapRecordInEnclosingClasses(
                        generateInternalRecord(recordSymbol), recordSymbol))
                    .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken))))
            .NormalizeWhitespace();

        context.AddSource(name, SourceText.From(resultCompilation.SyntaxTree.ToString(), Encoding.UTF8));
    }

    // this augments the internal syntax with ChildNodesAndTokens getter

    /// wraps (possibly inner) record in namespace and all containing types 
    private RecordDeclarationSyntax wrapRecordInEnclosingClasses(
        RecordDeclarationSyntax recordSyntax,
        ITypeSymbol recordSymbol
    ) {
        var wrapperSymbol = recordSymbol.ContainingType;
        while (wrapperSymbol != null) {
            /*
            if (wrapperSymbol.DeclaredAccessibility != Accessibility.Internal)
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "SDF0002",
                        "Invalid record accessibility",
                        "Record {0} is not internal",
                        "SDF",
                        DiagnosticSeverity.Error,
                        true
                    ),
                    null,
                    wrapperSymbol.Name
                ));
                */

            recordSyntax = RecordDeclaration(Token(SyntaxKind.RecordKeyword),
                    Identifier(getTypeNameWithGenericArguments(wrapperSymbol)))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword)))
                .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                .AddMembers(recordSyntax)
                .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));

            wrapperSymbol = wrapperSymbol.ContainingType;
        }

        return recordSyntax;
    }
}
