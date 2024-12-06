using AnkiScraping.Core.Operations;

namespace AnkiScraping.Core;

public readonly record struct ProviderKey<T>(string ProviderIdentifier)
{
    private string ProviderTypeName => typeof(T).Name;
    
    public override string ToString()
    {
        return $"{ProviderTypeName}: {ProviderIdentifier}";
    }
}

public readonly record struct ProviderQuery<T>()
{
    private string ProviderTypeName => typeof(T).Name;
    public OneOf<string, Any> ProviderIdentifier { get; init; } = new Any();

    public override string ToString()
    {
        return ProviderIdentifier.TryPickT0(out var providerId, out _)
            ? $"GET type='{ProviderTypeName}'&id='{providerId}'"
            : $"GET type='{ProviderTypeName}'&id=any";
    }
}