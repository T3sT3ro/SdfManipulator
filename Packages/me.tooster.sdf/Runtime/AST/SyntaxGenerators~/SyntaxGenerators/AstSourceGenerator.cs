using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

// TODO: handle nullable fields and generate nullable child getter with null filtering

// from: https://medium.com/@EnescanBektas/using-source-generators-in-the-unity-game-engine-140ff0cd0dc

[Generator]
public class AstSourceGenerator : ISourceGenerator {
    internal static readonly string SYNTAX_ATTRIBUTE_NAME = "Syntax";
    internal static readonly string INIT_ATTRIBUTE_NAME   = "Init";

    internal static readonly string ATTRIBUTES =
        @$"#nullable enable
using System;

/// Marks a partial record derived from Syntax for generation of constructors, properties and overrides
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
internal class {SYNTAX_ATTRIBUTE_NAME}Attribute : Attribute {{}}

/// Tells source generator to apply default value to the field using = new T()
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
internal class {INIT_ATTRIBUTE_NAME}Attribute : Attribute {{
    public Type? With {{ get; set; }} = null;
}}
";


    public void Initialize(GeneratorInitializationContext context) {
        context.RegisterForPostInitialization(i =>
            i.AddSource("SyntaxAttributes.g.cs", ATTRIBUTES));
        context.RegisterForSyntaxNotifications(() => new AstReceiver());
    }

    public void Execute(GeneratorExecutionContext context) {
        if (!(context.SyntaxContextReceiver is AstReceiver receiver))
            return;

        foreach (var kv in receiver.Records) {
            var recordSymbol = kv.Key;
            var usings = kv.Value;
            var qualifiedClassName = string.Join(".", getQualifiedNameParts(recordSymbol).Reverse());

            var resultCompilation = generateCompilationUnit(recordSymbol, context.Compilation)
                .WithUsings(List(new[]
                {
                    UsingDirective(QualifiedName(
                            QualifiedName(IdentifierName("System"), IdentifierName("Collections")),
                            IdentifierName("Generic")))
                        .WithUsingKeyword(Token(
                            TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))),
                            SyntaxKind.UsingKeyword, TriviaList())),
                    UsingDirective(QualifiedName(IdentifierName("System"), IdentifierName("Linq")))
                }))
                .AddUsings(usings.ToArray());

            context.AddSource($"{qualifiedClassName}.g.cs",
                SourceText.From(resultCompilation.NormalizeWhitespace().SyntaxTree.ToString(), Encoding.UTF8));
        }
    }
    // ------------------- GENERATION -------------------

    // gets reversed names up to AST namespace
    private static IEnumerable<string> getQualifiedNameParts(ITypeSymbol record) {
        ISymbol current = record;
        while (current is not INamespaceSymbol { Name: "AST" }) {
            yield return current.Name;

            current = current.ContainingSymbol;
        }
    }

    private static CompilationUnitSyntax generateCompilationUnit(ITypeSymbol recordSymbol,
        Compilation compilation) {
        var namespaceName = recordSymbol.ContainingNamespace.ToString();
        var namespaceDeclaration = NamespaceDeclaration(IdentifierName(namespaceName))
            .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
            .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));

        var currentRecordDeclaration = getPartialRecord(recordSymbol, compilation);
        var currentWrapperType = recordSymbol.ContainingType;
        while (currentWrapperType != null) {
            var accessibility = currentWrapperType.DeclaredAccessibility switch
            {
                Accessibility.Public    => SyntaxKind.PublicKeyword,
                Accessibility.Protected => SyntaxKind.ProtectedKeyword,
                _                       => throw new ArgumentOutOfRangeException()
            };

            currentRecordDeclaration = RecordDeclaration(
                    Token(SyntaxKind.RecordKeyword),
                    Identifier(getTypeNameWithGenericArguments(currentWrapperType)))
                .WithModifiers(
                    TokenList(Token(accessibility), Token(SyntaxKind.PartialKeyword)))
                .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                .AddMembers(currentRecordDeclaration)
                .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));

            currentWrapperType = currentWrapperType.ContainingType;
        }

        var resultCompilation = CompilationUnit()
            .WithMembers(SingletonList<MemberDeclarationSyntax>(
                namespaceDeclaration.AddMembers(currentRecordDeclaration)
            ));
        return resultCompilation;
    }

    private static IEnumerable<ITypeSymbol> getSelfAndOuterClasses(ITypeSymbol recordSymbol) {
        var currentClass = recordSymbol;
        while (currentClass != null) {
            yield return currentClass;

            currentClass = currentClass.ContainingType;
        }
    }

    /// returns fields requiring generating property getters 
    private static IEnumerable<IFieldSymbol> getOwnCompatibleFields(ITypeSymbol recordSymbol) {
        foreach (var field in recordSymbol.GetMembers().OfType<IFieldSymbol>()
                     .Where(isFieldCompatible))
            yield return field;
    }


    private static IEnumerable<IFieldSymbol> getInheritedCompatibleFields(
        ITypeSymbol recordSymbol) {
        var recordType = recordSymbol.BaseType;
        while (recordType != null && recordType.Name != "Syntax") {
            foreach (var field in recordType.GetMembers().OfType<IFieldSymbol>()
                         .Where(f => isTypeOfSyntaxOrTokenSubtype(f.Type)))
                yield return field;

            recordType = recordType.BaseType;
        }
    }

    // returns fields that had the [Init] attribute in this class.
    private static IEnumerable<IFieldSymbol> getFieldsForInitialization(
        IEnumerable<IFieldSymbol> ownFields,
        Compilation compilation) {
        var initAttribute =
            compilation.GetTypeByMetadataName($"{INIT_ATTRIBUTE_NAME}Attribute");
        foreach (var field in ownFields) {
            var hasInitAttribute = field.GetAttributes()
                .Any(ad =>
                    ad.AttributeClass?.Equals(initAttribute, SymbolEqualityComparer.Default)
                 ?? false);
            if (hasInitAttribute)
                yield return field;
        }
    }

    /// Returns compatible record fields for generation. Field is compatible if it's a private readonly field of
    /// base Syntax or Token type and it's name starts with underscore. 
    private static bool isFieldCompatible(IFieldSymbol field) {
        // must be private or protected
        if (field.DeclaredAccessibility != Accessibility.Private
         && field.DeclaredAccessibility != Accessibility.Protected)
            return false;

        // must be readonly and explicit
        if (!field.IsReadOnly || field.IsImplicitlyDeclared)
            return false;

        // must start with an underscore
        if (!field.Name.StartsWith("_"))
            return false;

        // some fields are generic type T with bound Syntax<Lang>, so it must be handled as well
        if (field.Type is ITypeParameterSymbol tps) {
            if (tps.ConstraintTypes.Any(isTypeOfSyntaxOrTokenSubtype))
                return true;
        }

        // must be of SyntaxOrToken<T> type
        if (!isTypeOfSyntaxOrTokenSubtype(field.Type))
            return false;

        return true;
    }

    private static bool isTypeOfSyntaxOrTokenSubtype(ITypeSymbol type) {
        var baseType = type.BaseType;
        while (baseType != null) {
            if (baseType.MetadataName == "SyntaxOrToken`1")
                return true;

            baseType = baseType.BaseType;
        }

        return false;
    }


    // generates properties like `public X x { get => _x; set => { _x = value with { Parent = this }; } }`
    private static IEnumerable<MemberDeclarationSyntax> generateProperties(
        IEnumerable<IFieldSymbol> ownFields) {
        var withExpression = WithExpression(
            IdentifierName("value"),
            InitializerExpression(
                SyntaxKind.WithInitializerExpression,
                SingletonSeparatedList<ExpressionSyntax>(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName("Parent"),
                        ThisExpression()))));

        foreach (var field in ownFields) {
            ExpressionSyntax fieldValueExpression = field.Type.NullableAnnotation != NullableAnnotation.Annotated
                ? withExpression
                : ConditionalExpression(BinaryExpression(SyntaxKind.EqualsExpression,
                        IdentifierName("value"),
                        LiteralExpression(SyntaxKind.NullLiteralExpression)),
                    IdentifierName("value"),
                    withExpression
                );


            yield return PropertyDeclaration(
                    IdentifierName(
                        field.Type.ToString()), // fully qualified name, possibly nullable or generic type argument
                    Identifier(field.Name.TrimStart('_')))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(AccessorList(List(
                    new[]
                    {
                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithExpressionBody(ArrowExpressionClause(IdentifierName(field.Name)))
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                        AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                            .WithExpressionBody(ArrowExpressionClause(AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                IdentifierName(field.Name),
                                fieldValueExpression)))
                            .WithSemicolonToken(
                                Token(SyntaxKind.SemicolonToken)),
                    })));
        }
    }

    private static IEnumerable<MemberDeclarationSyntax> generateToStringOverride() {
        yield return MethodDeclaration(
                PredefinedType(
                    Token(SyntaxKind.StringKeyword)),
                Identifier("ToString"))
            .WithModifiers(
                TokenList(
                    new[]
                    {
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.OverrideKeyword),
                    }))
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

    private static IEnumerable<MemberDeclarationSyntax> generateConstructor(
        ITypeSymbol recordSymbol, IEnumerable<IFieldSymbol> fieldsForInitialization) {
        yield return ConstructorDeclaration(recordSymbol.Name)
            .WithInitializer(ConstructorInitializer(SyntaxKind.BaseConstructorInitializer,
                ArgumentList()))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithBody(Block(generateCtorMemberInitializer(fieldsForInitialization)));
    }

    private static IEnumerable<ExpressionStatementSyntax> generateCtorMemberInitializer(
        IEnumerable<IFieldSymbol> fieldsForInitialization) {
        foreach (var field in fieldsForInitialization) {
            // if has [Init(typeof(X))], use = new X(); instead
            var initAttribute = field.GetAttributes()
                .First(ad => ad.AttributeClass?.Name == "InitAttribute");

            // case for [Init], results in: = new()
            ExpressionSyntax initExpression;
            // case for [Init(typeof(X)], results in: = new X()
            var namedAttributes = initAttribute.NamedArguments.ToDictionary(
                k => k.Key,
                v => v.Value);
            if (namedAttributes.TryGetValue("With", out var typeArg)
             && typeArg.Value != null) {
                initExpression =
                    ObjectCreationExpression(IdentifierName(typeArg.Value.ToString()))
                        .WithArgumentList(ArgumentList());
            } else {
                initExpression = ImplicitObjectCreationExpression();
            }

            yield return ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(
                            field.Name
                                .TrimStart(
                                    '_')), // without underscore because we route to property init
                        initExpression
                    ))
                .WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken));
        }
    }

    // generate ChildNodesAndTokens accessor, handle correct nullability
    private static PropertyDeclarationSyntax generateChildrenGetter(ITypeSymbol recordSymbol,
        IEnumerable<IFieldSymbol> inheritedAndOwnFields) {
        var langName = getQualifiedNameParts(recordSymbol).Last();

        TypeSyntax itemType = GenericName(Identifier("SyntaxOrToken"))
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName(langName))));

        var anyFieldNullable =
            inheritedAndOwnFields.Any(f => f.Type.NullableAnnotation == NullableAnnotation.Annotated);


        var arrayCreationExpressionSyntax = ArrayCreationExpression(
                ArrayType(anyFieldNullable ? NullableType(itemType) : itemType)
                    .WithRankSpecifiers(SingletonList(ArrayRankSpecifier(
                        SingletonSeparatedList<ExpressionSyntax>(
                            OmittedArraySizeExpression())))))
            .WithInitializer(InitializerExpression(
                SyntaxKind.ArrayInitializerExpression,
                SeparatedList<ExpressionSyntax>(
                    generateChildrenGetterMembers(inheritedAndOwnFields))));

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
    private static IEnumerable<SyntaxNodeOrToken>
        generateChildrenGetterMembers(IEnumerable<IFieldSymbol> ownFields) {
        foreach (var field in ownFields) {
            yield return IdentifierName(field.Name.TrimStart('_'));
            yield return Token(SyntaxKind.CommaToken); // trailing comma is OK
        }
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
    private static RecordDeclarationSyntax getPartialRecord(
        ITypeSymbol recordSymbol, Compilation compilation) {
        var ownFields = getOwnCompatibleFields(recordSymbol).ToList();
        var fieldsForInitialization =
            getFieldsForInitialization(ownFields, compilation).ToList();
        var inheritedFields = getInheritedCompatibleFields(recordSymbol).ToList();

        var hasDefaultConstructor = recordSymbol.GetMembers().OfType<IMethodSymbol>()
            .Any(m => m.MethodKind == MethodKind.Constructor
             && m.Parameters.Length == 0
             && m.DeclaredAccessibility == Accessibility.Public);

        var recordDeclaration = RecordDeclaration(
                Token(SyntaxKind.RecordKeyword),
                Identifier(getTypeNameWithGenericArguments(recordSymbol)))
            .WithModifiers(
                TokenList(
                    new[]
                    {
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.PartialKeyword)
                    }))
            .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
            .AddMembers(generateToStringOverride().ToArray())
            .AddMembers(generateProperties(ownFields).ToArray())
            .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));

        if (!hasDefaultConstructor)
            recordDeclaration =
                recordDeclaration.AddMembers(generateConstructor(recordSymbol, fieldsForInitialization).ToArray());


        // if it doesn't have the override and is not abstract, generate childrenAccessor
        var skipChildrenAccessor = recordSymbol.GetMembers("ChildNodesAndTokens").Any()
         || recordSymbol.IsAbstract;

        if (skipChildrenAccessor) return recordDeclaration;

        return recordDeclaration.AddMembers(generateChildrenGetter(recordSymbol,
            inheritedFields.Concat(ownFields)));
    }
}
