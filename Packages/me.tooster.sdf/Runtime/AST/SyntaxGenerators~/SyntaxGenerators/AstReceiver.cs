using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ITypeSymbol = Microsoft.CodeAnalysis.ITypeSymbol;

// This generator collects all partial classes with [Syntax] attribute
// Each partial class will have a property (with trailing underscore stripped) with getter and init setter
// where getter returns the field and setter assigns the field and uses the with { Parent = this }; syntax
// This is just a broad phase to sieve required types, it must be very minimal and performant.
// Runs once for many nodes
public class AstReceiver : ISyntaxContextReceiver {
    public HashSet<ITypeSymbol> Records { get; } = new();
    public HashSet<ITypeSymbol> Tokens  { get; } = new();
    public HashSet<ITypeSymbol> Trivia  { get; } = new();

    public HashSet<ITypeSymbol> Invalid { get; } = new();

    private ISymbol? syntaxAttribute;
    private ISymbol? tokenAttribute;
    private ISymbol? triviaAttribute;

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
        if (context.Node is not RecordDeclarationSyntax recordSyntax)
            return;

        syntaxAttribute ??= context.SemanticModel.Compilation.GetTypeByMetadataName(
            $"{AstSourceGenerator.SYNTAX_ATTRIBUTE_NAME}Attribute");
        tokenAttribute ??= context.SemanticModel.Compilation.GetTypeByMetadataName(
            $"{AstSourceGenerator.TOKEN_ATTRIBUTE_NAME}Attribute");
        triviaAttribute ??= context.SemanticModel.Compilation.GetTypeByMetadataName(
            $"{AstSourceGenerator.TRIVIA_ATTRIBUTE_NAME}Attribute");


        var recordSymbol = (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, recordSyntax) as ITypeSymbol)!;

        HashSet<ITypeSymbol>? targetList = null;

        if (isRecordAnnotatedWith(recordSymbol, syntaxAttribute!)) {
            targetList = isConstructedFrom(recordSymbol, "me.tooster.sdf.AST.Syntax.Syntax<Lang>") ? Records : Invalid;
        } else if (isRecordAnnotatedWith(recordSymbol, tokenAttribute!)) {
            targetList = isConstructedFrom(recordSymbol, "me.tooster.sdf.AST.Syntax.Token<Lang>") ? Tokens : Invalid;
        } else if (isRecordAnnotatedWith(recordSymbol, triviaAttribute!)) {
            targetList = isConstructedFrom(recordSymbol, "me.tooster.sdf.AST.Syntax.Trivia<Lang>") ? Trivia : Invalid;
        }

        // if it's not partial, bail
        if (targetList != null) {
            if (recordSyntax.Modifiers.All(mod => !mod.IsKind(SyntaxKind.PartialKeyword))
             || recordSymbol.DeclaredAccessibility != Accessibility.Public) {
                targetList = Invalid;
            }
        }

        targetList?.Add(recordSymbol);
    }


    // returns fields from inherited records which are of SyntaxOrToken subtype

    // must be internal partial and have [Syntax] attribute. TODO: maybe check later if it's not inheriting after [Syntax]

    internal static bool isRecordAnnotatedWith(ITypeSymbol recordSymbol, ISymbol attribute) {
        return recordSymbol
            .GetAttributes()
            .Any(ad => ad.AttributeClass?.Equals(attribute, SymbolEqualityComparer.Default) ?? false);
    }

    private bool isConstructedFrom(ITypeSymbol? typeSymbol, string constructedName) {
        while (typeSymbol != null) {
            if (typeSymbol.BaseType?.ConstructedFrom.ToString() == constructedName)
                return true;

            typeSymbol = typeSymbol.BaseType;
        }

        return false;
    }
}
