using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace me.tooster.sdf.AST.Generators {
    public static class Utils {
        /// returns a sequence of enclosing names for fully qualified until terminator is found (without generic type arguments)
        public static IEnumerable<string> qualifiedNameParts(this ITypeSymbol record,
            Predicate<ISymbol> terminatorPredicate
        ) {
            ISymbol current = record;
            while (current is not null && terminatorPredicate(current)) {
                yield return current.Name;

                current = current.ContainingSymbol;
            }
        }

        public static IEnumerable<string> qualifiedNameParts(this ITypeSymbol record,
            string terminator = AstSourceGenerator.AST_NAMESPACE
        ) =>
            record.qualifiedNameParts(current => current.Name != terminator);

        /// returns properties requiring generating property getters 
        public static IEnumerable<IPropertySymbol> OwnProperties(this ITypeSymbol recordSymbol, string? name = null) =>
            (name is null ? recordSymbol.GetMembers() : recordSymbol.GetMembers(name)).OfType<IPropertySymbol>();

        public static IEnumerable<IPropertySymbol> InheritedProperties(this ITypeSymbol recordSymbol, string? name = null) {
            var parent = recordSymbol.BaseType;
            while (parent != null) {
                foreach (var property in (name is null ? parent.GetMembers() : parent.GetMembers(name)).OfType<IPropertySymbol>())
                    yield return property;

                parent = parent.BaseType;
            }
        }

        /// Returns true if record property is a AST-generator compatible.
        /// Property is compatible if it's public, explicit and either derived from SyntaxOrToken type or annotated 
        public static bool isPropertyCompatible(this IPropertySymbol property) {
            // must be public and explicit
            if (property.DeclaredAccessibility != Accessibility.Public || property.IsImplicitlyDeclared)
                return false;

            // must be of subtype annotated with [Syntax] or [Token]
            return property.Type.BaseTypes().Any(type =>
                (type as INamedTypeSymbol)?.ConstructedFrom.ToDisplayString() // TODO: migrate to Name comparison?
             == $"{AstSourceGenerator.ROOT_NAMESPACE}.Syntax.SyntaxOrToken<Lang>"
             || type.GetAttributes().Any(ad =>
                    ad.AttributeClass?.Name
                        is AstSourceGenerator.SYNTAX_ATTRIBUTE_NAME
                        or AstSourceGenerator.TOKEN_ATTRIBUTE_NAME
                        or AstSourceGenerator.TRIVIA_ATTRIBUTE_NAME)
            );
        }

        /// returns if type (possibly generic), any of it's type bounds or any of it's base types match predicate
        [Obsolete("this is not used yet, but I can migrate to it later")]
        public static bool checkTypeChain(ITypeSymbol? type, Predicate<ITypeSymbol> predicate) {
            if (type is ITypeParameterSymbol tps && tps.ConstraintTypes.Any(t => checkTypeChain(t, predicate)))
                return true;

            while (type != null) {
                if (predicate(type))
                    return true;

                type = type.BaseType;
            }

            return false;
        }

        public static IEnumerable<ITypeSymbol> BaseTypes(this ITypeSymbol type,
            bool includeSelf = true,
            bool descendTypeConstraints = true
        ) {
            if (includeSelf) yield return type;

            if (descendTypeConstraints && type is ITypeParameterSymbol tps)
                foreach (var baseType in tps.ConstraintTypes.SelectMany(constraint => constraint.BaseTypes(true, true)))
                    yield return baseType;
            else {
                while (type.BaseType != null)
                    yield return type = type.BaseType;
            }
        }

        /// returns type name with generic arguments and nullable annotation (generic args are fully qualified)
        public static string getTypeNameWithGenericArguments(this ITypeSymbol typeSymbol,
            bool withEnclosingRecordParts = false) {
            var sb = new StringBuilder(typeSymbol.Name);
            if (typeSymbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol) {
                sb.Append("<");
                sb.Append(string.Join(", ",
                    namedTypeSymbol.TypeArguments.Select(t => t.getTypeNameWithGenericArguments())));
                sb.Append(">");
            }

            if (typeSymbol.NullableAnnotation == NullableAnnotation.Annotated)
                sb.Append("?");

            if (withEnclosingRecordParts && typeSymbol.IsRecord && typeSymbol.ContainingSymbol is ITypeSymbol parent
             && parent.IsRecord) {
                sb.Insert(0, ".");
                sb.Insert(0, parent.getTypeNameWithGenericArguments());
            }

            return sb.ToString();
        }

        public static string getFullyQualifiedTypeNameWithGenerics(this ISymbol typeSymbol,
            string terminator,
            out List<string> genericParameters
        ) {
            var parts = new List<string>();
            var current = typeSymbol;
            genericParameters = new List<string>();
            while (current != null && current.Name != terminator) {
                if (current is ITypeSymbol ts)
                    parts.Add(ts.getTypeNameWithGenericArguments());
                else
                    parts.Add(current.Name);
                if (current is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol) {
                    genericParameters.AddRange(
                        namedTypeSymbol.TypeArguments.Select(p => p.getTypeNameWithGenericArguments()));
                }

                current = current.ContainingSymbol;
            }

            return string.Join(".", parts.Reverse<string>());
        }

        /// wraps (possibly inner) record in namespace and all containing types 
        public static RecordDeclarationSyntax wrapRecordInEnclosingClasses(this RecordDeclarationSyntax recordSyntax,
            ITypeSymbol recordSymbol,
            bool isInternal
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

                var modifiers = TokenList(Token(SyntaxKind.PublicKeyword));
                if (wrapperSymbol.IsAbstract) modifiers = modifiers.Add(Token(SyntaxKind.AbstractKeyword));
                modifiers = modifiers.Add(Token(SyntaxKind.PartialKeyword));

                recordSyntax = RecordDeclaration(Token(SyntaxKind.RecordKeyword),
                        Identifier(wrapperSymbol.getTypeNameWithGenericArguments()))
                    .WithModifiers(modifiers)
                    .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                    .AddMembers(recordSyntax)
                    .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));


                wrapperSymbol = wrapperSymbol.ContainingType;
            }

            return recordSyntax;
        }

        // returns the lang name - namespace after "AST" namespace
        public static string getLangName(this ITypeSymbol recordSymbol) => recordSymbol.qualifiedNameParts().Last();

        internal static bool isAnnotated(this ITypeSymbol type, ISymbol attribute) {
            return type.GetAttributes().Any(ad =>
                ad.AttributeClass?.Equals(attribute, SymbolEqualityComparer.Default) ?? false);
        }

        public static bool isAstToken(this ITypeSymbol pType) {
            return pType.BaseTypes().Any(type =>
                (type as INamedTypeSymbol)?.ConstructedFrom.ToDisplayString()
             == $"{AstSourceGenerator.ROOT_NAMESPACE}.Syntax.Token<Lang>"
             || type.GetAttributes().Any(ad => ad.AttributeClass?.Name is AstSourceGenerator.TOKEN_ATTRIBUTE_NAME)
            );
        }

        public static MemberDeclarationSyntax? generateToStringOverride(ITypeSymbol recordSymbol) {
            return recordSymbol.GetMembers("ToString").Any(m => !m.IsImplicitlyDeclared)
                ? null
                : ParseMemberDeclaration("public override string ToString() => base.ToString();");
        }
        
        public static ITypeSymbol? languageAgnosticType(this ITypeSymbol typeSymbol) =>
            typeSymbol.BaseTypes().FirstOrDefault(t => t.ContainingNamespace.ToString().Contains("AST.Syntax"));
    }
}
