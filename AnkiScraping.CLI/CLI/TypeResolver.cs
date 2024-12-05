using Spectre.Console.Cli;

namespace AnkiScraping.Host.CLI;

public sealed class TypeResolver(IServiceProvider serviceProvider) : ITypeResolver, IDisposable
{
    public object? Resolve(Type? type)
    {
        if (type == null)
        {
            return null;
        }

        return serviceProvider.GetService(type);
    }

    public void Dispose()
    {
        if (serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}