namespace AnkiScraping.Core;

public readonly record struct KanjiProviderNotFound<T>(ProviderQuery<T> ProviderId);