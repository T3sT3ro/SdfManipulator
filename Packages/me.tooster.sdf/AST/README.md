# The work in this package was strongly inspired by Roslyn

To see an explanation of nodes, go to [Roslyn Docs](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/work-with-syntax)

## TL;DR

- `Tree`: Container for syntax tree. Has `Root` property to access the root node.
- `Syntax`: non-terminal node, like `BinaryExpression`. Has methods for getting children nodes. Descendants have strongly typed specialized properties like `Left`, `Operator`, `Right` for `BinaryExpression`.
- `Token`: terminal node, never a parent of any other token.
  - compared to Roslyn, they don't have `Kind` enum property but are distinguished by their class identity
  - Tokens represent for example punctuation characters, string literals, numeric literals, identifiers, keywords etc.
- `Trivia`: comments, whitespaces, newlines, etc. Never a parent of any other trivia. Attached to tokens and have `someTrivia.Token` property to access it.
- Compared to roslyn, there is for now no distinction between green (internal syntax) and red nodes (public syntax). This may change in the future when source generators are added
o the package. Instead, a `Syntax
- There are naming similarities like `Identifier` and `IdentifierToken`. In this case `Identifier` is a `Syntax` holding `IdentifierToken`, a `SyntaxToken` holding the actual text value accessible with `Text` property.

[Here is a doc that includes more thorough but still concise explanation of roslyn concepts](https://github.com/xamarin/Workbooks/blob/master/csharp/roslyn/roslyn-syntax-trees.workbook/index.workbook)

# Tokens — enums or not?

In Roslyn they have a `SyntaxKind` enum which is a union of all possible tokens, expressions and generally language structures. This is good and more performant than this implementation, but this one doesn't that much boilerplate.

It's a tradeoff between hardcoded `floatNxM` for every combination and flexibility in creating tokens matching regexes. It is also less memory efficient (each token has it's class metadata, is an instance of a class, etc.)

# Trivia - changes

I Noticed they have Leading and Trailing trivia in Roslyn, which adds a bunch o edge cases (but helps in heuristics, for example about which trivia belongs to which token). Typescript has only leading trivia but they add special EOF token in every program to attach last trivia in in file. 

I added an indirection layer - tokens point to trivia lists instead of simple trivia. either list is empty, is a singleton list or contains many (possibly structured) trivia. This seems to simplify a conceptual model a bit.

# Syntax package

These classes seem convoluted at first glance, but they provide type-safe base classes and abstractions of key syntax defining structures. Instead of operating on an abstract base classes like `SyntaxNode`, each overload returns language-specific type of the token, trivia, syntax node and syntax tree. 