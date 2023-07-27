# The work in this package was strongly inspired by Roslyn

To see an explanation of nodes, go to [Roslyn Docs](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/work-with-syntax)

## TL;DR

- `SyntaxTree`: Container for syntax tree. Has `Root` property to access the root node.
- `SyntaxNode`: non-terminal node, like `BinaryExpression`. Has methods for getting children nodes. Descendants have strongly typed specialized properties like `Left`, `Operator`, `Right` for `BinaryExpression`.
- `SyntaxToken`: terminal nodes, never a parent of any other token.
  - They have `Kind` property
  - Tokens include punctuation characters, string literals, numeric literals, identifiers, and keywords.
- `SyntaxTrivia`: comments, whitespaces, newlines, etc. Never a parent of any other trivia. Attached to tokens and have `someTrivia.Token` property to access it.

- There are things like `IdentifierName` and `IdentifierToken`. In this case `IdentifierName` is a `SyntaxNode` which holds `IdentifierToken`, a `SyntaxToken` holding the actual value of the identifier accessible with `Text` property, which is abstracted away into `Token`, like a variable name

[Here is a doc that includes more thorough but still concise explanation of roslyn concepts](https://github.com/xamarin/Workbooks/blob/master/csharp/roslyn/roslyn-syntax-trees.workbook/index.workbook)

# Tokens — enums or not?

In Roslyn they have a `SyntaxKind` enum which is a union of all possible tokens, expressions and generally language structures. Would it be better to have a separate class per token type, or if not, separate enum per some kind of class, like Keywords, Expressions, Tokens, Literals etc?

It looks very verbose to have something like a bunch of `floatNxM` kinds while they can be simply created from a float and an arity. But it also looks like a lot of boilerplate code to create a separate class for each token type, and what about instantiation? How to handle `Literal` and `Identifier`?

# Syntax package

These classes seem convoluted at first glance, but they provide type-safe base classes and abstractions of key syntax defining structures. Instead of operating on an abstract base classes like `SyntaxNode`, each overload returns language-specific type of the token, trivia, syntax node and syntax tree. 