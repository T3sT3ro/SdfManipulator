# What?
This project implements source generators for AST nodes. Built dll is then copied to the appropriate place for the Package to use.

# How does it work?
It depends on the Roslyn compiler platform and it's syntax builders to create appropriate source files

# What it should support:
- generating appropriate helpers for AST nodes annotated with `[Ast]` attribute
  - declared classes must define `private readonly` fields starting with underscore `_` and having a subtype of `ISyntaxOrToken<T>`
  - for each such field, an appropriate property without underscode and with `{get; init;}` is created. `init` setter triggers setting a parent for the node
  - It should handle nullable types
  - It should override `ToString()` in classes to call base ISyntaxOrToken `ToString()`. This is required, because `ISyntaxOrToken` has a `Parent` property which would normaly result in a stack overflow exception (and crashing everything) when printed with `ToString()`. This is also needed in C#9 because the support for `sealed ToString()` was added only in C#10
  - Define constructors setting default values for fields annotated with `[AstDefault]`
  - Use default-constructible types when used with `[AstDefault(typeof(X))]` to initialize members like `_x = new X() with { Parent = this; }`
  - Call base constructors (to initialize abstract base classes members)
  - Generate appropriate `ChildNodesAndTokens` overrides with correct nullability
  - generates get only property `Parent` and internal, init only property `InitParent` which writes to the field `_parent`. `Parent` just reroutes to `InitParent` getter 

It should return a file looking like that:

```csharp
<usings>
    
namespace AST.<Lang>.<Dirs*> {
    public partial record <ParentRecords*> {
        public partial record <SyntaxName> {
            public <SyntaxOrTokenType> <member> { get => _<member>; set => _<member> = value with { Parent = this }; }
            private readonly ISyntax<<Lang>> _parent;
            public ISyntax<<Lang>> Parent => _parent;
            internal ISyntax<<Lang>> RedParent { init => _parent = value }
              
            public <SyntaxName>(){
                _member = new();
            }
              
            public IReadOnlyList<SyntaxOrToken<<Lang>>> ChildNodesAndTokens => new SyntaxOrTokenList[]<?> 
              { _member }<.Where(x => x is not null).Select(x => x!).ToList>
            
            public override string ToString() => base.ToString();
        }
    }
} 
```

