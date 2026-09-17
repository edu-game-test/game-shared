// Polyfill so `init` accessors compile on netstandard2.1. Unity ships its own copy; `internal` avoids a clash.
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
