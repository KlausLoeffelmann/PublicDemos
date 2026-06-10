namespace NETC64.ConceptApp;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
internal class PointerSizeTypeAttribute(PointerSizeTypes pointerSizeType) : Attribute
{
    public PointerSizeTypes PointerSizeType { get; } = pointerSizeType;
}

[AttributeUsage(AttributeTargets.Method)]
internal class PreferZeropageRegistersAttribute() : Attribute
{
}

internal enum PointerSizeTypes
{
    Word = 2,
    DWord = 4
}
