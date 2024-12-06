namespace AnkiScraping.Core;

public readonly record struct ProviderNotFound<T>(ProviderQuery<T> Query);