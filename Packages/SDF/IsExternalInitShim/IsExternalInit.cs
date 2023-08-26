// This is needed to solve compiler errors on current .NET platform and C# version in Unity.
// without it `init` keyword and primary record constructors don't work
namespace System.Runtime.CompilerServices {
    public static class IsExternalInit { }
}
