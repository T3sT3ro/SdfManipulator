using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace me.tooster.sdf.AST.Generators {
    public class Utils {
        /// returns a sequence of enclosing names for fully qualified until terminator is found namespace (without generic type arguments)
        public static IEnumerable<string> getQualifiedNameParts(
            ITypeSymbol record,
            string terminator = AstSourceGenerator.AST_NAMESPACE
        ) {
            ISymbol current = record;
            while (current is not null && current.Name != terminator) {
                yield return current.Name;

                current = current.ContainingSymbol;
            }
        }

        /// returns properties requiring generating property getters 
        public static IEnumerable<IPropertySymbol> getOwnProperties(ITypeSymbol recordSymbol) =>
            recordSymbol.GetMembers().OfType<IPropertySymbol>().Where(isPropertyCompatible);

        public static IEnumerable<IPropertySymbol> getInheritedProperties(ITypeSymbol recordSymbol) {
            var recordType = recordSymbol.BaseType;
            while (recordType != null) {
                foreach (var property in recordType.GetMembers().OfType<IPropertySymbol>().Where(isPropertyCompatible))
                    yield return property;

                recordType = recordType.BaseType;
            }
        }

        /// Returns compatible record fields for generation. Field is compatible if it's a private readonly field of
        /// base Syntax or Token type and it's name starts with underscore. 
        public static bool isPropertyCompatible(IPropertySymbol property) {
            // must be public and explicit
            if (property.DeclaredAccessibility != Accessibility.Public || property.IsImplicitlyDeclared)
                return false;

            // must be of subtype annotated with [Syntax] or [Token]
            return baseTypes(property.Type).Any(type =>
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

        public static IEnumerable<ITypeSymbol> baseTypes(
            ITypeSymbol type,
            bool includeSelf = true,
            bool descendTypeConstraints = true
        ) {
            if (includeSelf) yield return type;

            if (descendTypeConstraints && type is ITypeParameterSymbol tps)
                foreach (var constraint in tps.ConstraintTypes)
                    foreach (var baseType in baseTypes(constraint, true, true))
                        yield return baseType;
            else {
                while (type.BaseType != null)
                    yield return type = type.BaseType;
            }
        }

        /*
         * backlog:
         * - refactor into checktypeChain
         * - move from `isConstructedFrom` and `isAnnotated`
         */

        /// returns type name with generic arguments and nullable annotation (generic args are fully qualified)
        public static string getTypeNameWithGenericArguments(ITypeSymbol typeSymbol) {
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

        /// wraps (possibly inner) record in namespace and all containing types 
        public static RecordDeclarationSyntax wrapRecordInEnclosingClasses(
            RecordDeclarationSyntax recordSyntax,
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
                        Identifier(getTypeNameWithGenericArguments(wrapperSymbol)))
                    .WithModifiers(modifiers)
                    .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                    .AddMembers(recordSyntax)
                    .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));


                wrapperSymbol = wrapperSymbol.ContainingType;
            }

            return recordSyntax;
        }

        // returns the lang name - namespace after "AST" namespace
        public static string getLangName(ITypeSymbol recordSymbol) => getQualifiedNameParts(recordSymbol).Last();

        internal static bool isAnnotated(ITypeSymbol type, ISymbol attribute) {
            return type.GetAttributes().Any(ad =>
                ad.AttributeClass?.Equals(attribute, SymbolEqualityComparer.Default) ?? false);
        }
    }
}