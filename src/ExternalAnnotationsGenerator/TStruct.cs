using System.Diagnostics.CodeAnalysis;

namespace ExternalAnnotationsGenerator
{
    /// <summary>
    /// Type used as a replacement for a real type in methods requiring specifically a struct.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct TStruct
    {
    }
}
