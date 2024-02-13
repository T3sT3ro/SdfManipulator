# What?
This project implements source generators for AST nodes. Built dll is then copied to the appropriate place for the Package to use.

# How does it work?
It depends on the Roslyn compiler platform and it's syntax builders to create appropriate source files

# What it should support:
- generating appropriate helpers for AST nodes annotated with `[Syntax]` attribute
  - [x] declared classes must define `private readonly` fields starting with underscore `_` and having a subtype of `ISyntaxOrToken<T>` (or generic bound).
  - [x] for each such field, an appropriate property without underscore and with `{get; init;}` is created.
  - [x] `init` setter assigns parent node on assignment, which is similar to red-green tree in roslyn but without internal syntax nodes.
  - [x] properties should handle nullability correctly
  - [x] It should handle nullable types and appropriately filter out null types from the `ChildNodesAndTokens` property.
  - [x] It should override `ToString()` in classes to call base ISyntaxOrToken `ToString()`. This is required for records, because `ISyntaxOrToken` has a `Parent` property which would normally cause stack overflow exception (and crashing everything, even debugger) when printing with default synthesized record's `ToString()`. This is unfortunately needed in C#9 because the support for `sealed ToString()` was added only in C#10 for records.
  - [x] Define default constructor setting default values for fields annotated with `[Init]` attribute
  - [x] Use user provided types when used with `[Init(With = typeof(X))]` to initialize members like `_x = new X() with { Parent = this; }`
  - [x] Call base constructors (to initialize abstract base classes members)
  - [x] Generate appropriate `ChildNodesAndTokens` overrides with correct nullability
  - [ ] generates get only property `Parent` and internal/protected, init only property `InitParent` which writes to the field `_parent`. `Parent` just reroutes to `InitParent` getter 

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


# TODO:

- [ ] Correct the file links to the AST package using solutions from [this answer](https://stackoverflow.com/questions/1292351/including-content-files-in-csproj-that-are-outside-the-project-cone). 
  - To do that, RMB on the assembly -> edit (or edit `.csproj` by hand) and edit link in there
- [ ] correct the logic of generating syntax by using formatters with official format (like fully qualified type) from [this answer](https://stackoverflow.com/questions/23305594/getting-the-fully-qualified-name-of-a-type-from-a-typeinfo-object/23314956#23314956)
- [ ] add csproj to git, exclude it from .gitignore
