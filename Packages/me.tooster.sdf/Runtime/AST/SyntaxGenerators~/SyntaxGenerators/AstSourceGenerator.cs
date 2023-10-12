using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

// from: https://medium.com/@EnescanBektas/using-source-generators-in-the-unity-game-engine-140ff0cd0dc

[Generator]
public partial class AstSourceGenerator : ISourceGenerator {
    internal static readonly string SYNTAX_ATTRIBUTE_NAME = "AstSyntax";
    internal static readonly string TOKEN_ATTRIBUTE_NAME  = "AstToken";
    internal static readonly string TRIVIA_ATTRIBUTE_NAME = "AstTrivia";

    internal static readonly string ATTRIBUTES =
        @$"#nullable enable
using System;

/*
 * Below markers are used in by the source generator to generate Red AST nodes (lazy, with parents)  
 */

/// Marks Syntax for generation
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
internal class {SYNTAX_ATTRIBUTE_NAME}Attribute : Attribute {{}}

/// Marks Token for generation
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
internal class {TOKEN_ATTRIBUTE_NAME}Attribute : Attribute {{}}

/// Marks Trivia for generation
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
internal class {TRIVIA_ATTRIBUTE_NAME}Attribute : Attribute {{}}
";

    private Compilation               compilation;
    private GeneratorExecutionContext context;

    public void Initialize(GeneratorInitializationContext context) {
        context.RegisterForPostInitialization(i => i.AddSource("SyntaxAttributes.g.cs", ATTRIBUTES));
        context.RegisterForSyntaxNotifications(() => new AstReceiver());
    }

    public void Execute(GeneratorExecutionContext context) {
        if (!(context.SyntaxContextReceiver is AstReceiver receiver))
            return;

        compilation = context.Compilation;
        this.context = context;

        foreach (var invalid in receiver.Invalid) {
            report(AstDiagnostic.BAD_ATTRIBUTE_USAGE, invalid);
        }

        foreach (var recordSymbol in receiver.Records) GenerateSyntaxClasses(recordSymbol);
    }
    // ------------------- GENERATION -------------------

    public enum AstDiagnostic {
        BAD_ATTRIBUTE_USAGE,
    }

    public void report(AstDiagnostic diagnosticKind, ISymbol symbol) {
        var diagnostic = diagnosticKind switch
        {
            AstDiagnostic.BAD_ATTRIBUTE_USAGE => Diagnostic.Create(new DiagnosticDescriptor(
                "AST0001", "Invalid attribute usage",
                $"Record {symbol.Name} with [Syntax]/[Token]/[Trivia] attribute must be public, partial and inherit from Syntax/Token/Trivia",
                "AST", DiagnosticSeverity.Error, true), Location.None),
            _ => throw new ArgumentOutOfRangeException(nameof(diagnosticKind), diagnosticKind, null),
        };

        context.ReportDiagnostic(diagnostic);
    }

    // gets reversed names up to AST namespace without generic type arguments
    private static IEnumerable<string> getQualifiedNameParts(ITypeSymbol record) {
        ISymbol current = record;
        while (current is not INamespaceSymbol { Name: "AST" }) {
            yield return current.Name;

            current = current.ContainingSymbol;
        }
    }

    /// returns properties requiring generating property getters 
    private static IEnumerable<IPropertySymbol> getOwnProperties(ITypeSymbol recordSymbol) {
        return recordSymbol.GetMembers().OfType<IPropertySymbol>().Where(isPropertyCompatible);
    }


    private static IEnumerable<IPropertySymbol> getInheritedProperties(ITypeSymbol recordSymbol) {
        var recordType = recordSymbol.BaseType;
        while (recordType != null) {
            ;
            foreach (var property in recordType.GetMembers().OfType<IPropertySymbol>().Where(isPropertyCompatible))
                yield return property;

            recordType = recordType.BaseType;
        }
    }

    /// Returns compatible record fields for generation. Field is compatible if it's a private readonly field of
    /// base Syntax or Token type and it's name starts with underscore. 
    private static bool isPropertyCompatible(IPropertySymbol property) {
        // must be private or protected
        if (property.DeclaredAccessibility != Accessibility.Internal)
            return false;

        // must be explicit
        if (!property.IsImplicitlyDeclared)
            return false;

        // must be of subtype annotated with [Syntax] or [Token]
        return isTypeAstAnnotated(property.Type);
    }

    // returns if type (possibly generic) is of base type annotated with [Syntax] or [Token]
    private static bool isTypeAstAnnotated(ITypeSymbol? type) {
        if (type is ITypeParameterSymbol tps && tps.ConstraintTypes.Any(isTypeAstAnnotated))
            return true;

        while (type != null) {
            if (type.GetAttributes().Any(ad =>
                    ad.AttributeClass?.Name == SYNTAX_ATTRIBUTE_NAME
                 || ad.AttributeClass?.Name == TOKEN_ATTRIBUTE_NAME
                 || ad.AttributeClass?.Name == TRIVIA_ATTRIBUTE_NAME))
                return true;

            type = type.BaseType;
        }

        return false;
    }

    private static IEnumerable<MemberDeclarationSyntax> generateToStringOverride() {
        yield return MethodDeclaration(
                PredefinedType(
                    Token(SyntaxKind.StringKeyword)),
                Identifier("ToString"))
            .WithModifiers(
                TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
            .WithExpressionBody(
                ArrowExpressionClause(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            BaseExpression(),
                            IdentifierName("ToString")))))
            .WithSemicolonToken(
                Token(SyntaxKind.SemicolonToken));
    }

    // generate ChildNodesAndTokens accessor, handle correct nullability
    private static PropertyDeclarationSyntax generateChildrenGetter(
        ITypeSymbol recordSymbol,
        IEnumerable<IPropertySymbol> inheritedAndOwnProperties
    ) {
        var langName = getQualifiedNameParts(recordSymbol).Last();

        TypeSyntax itemType = GenericName(Identifier("SyntaxOrToken"))
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName(langName))));

        var anyFieldNullable =
            inheritedAndOwnProperties.Any(f => f.Type.NullableAnnotation == NullableAnnotation.Annotated);


        var arrayCreationExpressionSyntax = ArrayCreationExpression(
                ArrayType(anyFieldNullable ? NullableType(itemType) : itemType)
                    .WithRankSpecifiers(SingletonList(ArrayRankSpecifier(
                        SingletonSeparatedList<ExpressionSyntax>(
                            OmittedArraySizeExpression())))))
            .WithInitializer(InitializerExpression(
                SyntaxKind.ArrayInitializerExpression,
                SeparatedList<ExpressionSyntax>(
                    generateChildrenGetterMembers(inheritedAndOwnProperties))));

        return PropertyDeclaration(GenericName(Identifier("IReadOnlyList"))
                    .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(itemType))),
                Identifier("ChildNodesAndTokens"))
            .WithModifiers(
                TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
            .WithExpressionBody(
                ArrowExpressionClause(
                    !anyFieldNullable
                        ? arrayCreationExpressionSyntax
                        : InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                            InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                    InvocationExpression(MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            arrayCreationExpressionSyntax,
                                            IdentifierName("Where")))
                                        .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(
                                            SimpleLambdaExpression(Parameter(Identifier("n")))
                                                .WithExpressionBody(IsPatternExpression(IdentifierName("n"),
                                                    UnaryPattern(ConstantPattern(LiteralExpression(
                                                        SyntaxKind.NullLiteralExpression))))))))),
                                    IdentifierName("Select")))
                                .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(
                                    SimpleLambdaExpression(Parameter(Identifier("n")))
                                        .WithExpressionBody(PostfixUnaryExpression(
                                            SyntaxKind.SuppressNullableWarningExpression,
                                            IdentifierName("n"))))))),
                            IdentifierName("ToList")))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }

    /// generates contents of the ChildNodesAndTokens { a, b, c }; 
    private static IEnumerable<SyntaxNodeOrToken> generateChildrenGetterMembers(
        IEnumerable<IPropertySymbol> allProperties
    ) {
        foreach (var field in allProperties) {
            yield return IdentifierName(field.Name);
            yield return Token(SyntaxKind.CommaToken); // trailing comma is OK
        }
    }

    private static IEnumerable<LocalDeclarationStatementSyntax> mapperFieldDeclarations(
        IEnumerable<IPropertySymbol> inheritedAndOwnFields
    ) {
        foreach (var prop in inheritedAndOwnFields) {
            yield return LocalDeclarationStatement(
                VariableDeclaration(IdentifierName(
                        Identifier(
                            TriviaList(),
                            SyntaxKind.VarKeyword,
                            "var",
                            "var",
                            TriviaList())))
                    .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(prop.Name))
                        .WithInitializer(EqualsValueClause(InvocationExpression(
                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("mapper"),
                                    IdentifierName("Map")))
                            .WithArgumentList(ArgumentList(SingletonSeparatedList(
                                Argument(CastExpression(
                                    IdentifierName("dynamic"),
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        ThisExpression(),
                                        IdentifierName(prop.Name))))))))))));
        }
    }

    private static InvocationExpressionSyntax referencesEqualCheck(IPropertySymbol property) {
        return InvocationExpression(IdentifierName("ReferenceEquals"))
            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
                new SyntaxNodeOrToken[]
                {
                    Argument(IdentifierName(property.Name)),
                    Token(SyntaxKind.CommaToken),
                    Argument(MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        ThisExpression(),
                        IdentifierName(property.Name)))
                })));
    }

    private static IfStatementSyntax mapperIfStatement(
        IEnumerable<IPropertySymbol> inheritedAndOwnFields
    ) {
        ExpressionSyntax? testExpression = null;
        foreach (var prop in inheritedAndOwnFields) {
            testExpression = testExpression == null
                ? referencesEqualCheck(prop)
                : BinaryExpression(SyntaxKind.LogicalAndExpression, testExpression, referencesEqualCheck(prop));
        }

        return IfStatement(
            testExpression ?? LiteralExpression(SyntaxKind.TrueLiteralExpression),
            ReturnStatement(ThisExpression()));
    }

    private static ReturnStatementSyntax mapperObjectCreationExpression(
        ITypeSymbol recordSymbol,
        IEnumerable<IPropertySymbol> inheritedAndOwnProperties
    ) {
        return ReturnStatement(ObjectCreationExpression(IdentifierName(getTypeNameWithGenericArguments(recordSymbol)))
            .WithInitializer(InitializerExpression(SyntaxKind.ObjectInitializerExpression,
                SeparatedList<ExpressionSyntax>(
                    inheritedAndOwnProperties.Select(field =>
                        AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                            IdentifierName(field.Name),
                            IdentifierName(field.Name)))))));
    }

    private static IEnumerable<StatementSyntax> mapperFunctionStatements(
        ITypeSymbol recordSymbol,
        IEnumerable<IPropertySymbol> inheritedAndOwnProperties
    ) {
        foreach (var st in mapperFieldDeclarations(inheritedAndOwnProperties))
            yield return st;

        yield return mapperIfStatement(inheritedAndOwnProperties);

        yield return mapperObjectCreationExpression(recordSymbol, inheritedAndOwnProperties);
    }

    private static MethodDeclarationSyntax generateMapper(
        ITypeSymbol recordSymbol,
        List<IPropertySymbol> inheritedAndOwnProperties
    ) {
        var langName = getQualifiedNameParts(recordSymbol).Last();
        return MethodDeclaration(GenericName(Identifier("Syntax"))
                    .WithTypeArgumentList(
                        TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName(langName)))),
                Identifier("MapWith"))
            .WithModifiers(
                TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
            .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier("mapper"))
                .WithType(GenericName(Identifier("Mapper"))
                    .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(
                        IdentifierName(langName))))))))
            .WithBody(Block(mapperFunctionStatements(recordSymbol, inheritedAndOwnProperties)));
    }

    // returns type name with generic arguments and nullable annotation (generic args are fully qualified)
    private static string getTypeNameWithGenericArguments(ITypeSymbol typeSymbol) {
        var sb = new StringBuilder(typeSymbol.Name);
        if (typeSymbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol) {
            sb.Append("<");
            sb.Append(string.Join(", ",
                namedTypeSymbol.TypeArguments.Select(getTypeNameWithGenericArguments)));
            sb.Append(">");
        }

        if (typeSymbol.NullableAnnotation == NullableAnnotation.Annotated)
            sb.Append("?");

        return sb.ToString();
    }

    /// creates partial record with properties, constructor, toString and children getter wrapped in inner classes
    private static RecordDeclarationSyntax generateInternalRecord(ITypeSymbol recordSymbol) {
        var ownProperties = getOwnProperties(recordSymbol).ToList();
        var inheritedProperties = getInheritedProperties(recordSymbol).ToList();

        var recordDeclaration = RecordDeclaration(Token(SyntaxKind.RecordKeyword),
                Identifier(getTypeNameWithGenericArguments(recordSymbol)))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword)))
            .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
            .AddMembers(generateToStringOverride().ToArray())
            .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));

        // if it doesn't have the override and is not abstract, generate childrenAccessor

        var inheritedAndOwnProperties = inheritedProperties.Concat(ownProperties).ToList();

        // skip children accessor if it exists
        if (!(recordSymbol.GetMembers("ChildNodesAndTokens").Any() || recordSymbol.IsAbstract))
            recordDeclaration = recordDeclaration
                .AddMembers(generateChildrenGetter(recordSymbol, inheritedAndOwnProperties));

        // skip MapWith if it exists
        if (!(recordSymbol.GetMembers("MapWith").Any() || recordSymbol.IsAbstract))
            recordDeclaration = recordDeclaration.AddMembers(generateMapper(recordSymbol, inheritedAndOwnProperties));

        return recordDeclaration;
    }
}
