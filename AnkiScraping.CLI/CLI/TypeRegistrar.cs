using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace AnkiScraping.Host.CLI;

public sealed class TypeRegistrar(IServiceCollection services) : ITypeRegistrar
{
    public ITypeResolver Build()
    {
        return new TypeResolver(services.BuildServiceProvider());
    }

    public void Register(Type service, Type implementation)
    {
#pragma warning disable IL2067
        services.AddSingleton(service, implementation);
#pragma warning restore IL2067
    }

    public void RegisterInstance(Type service, object implementation)
    {
        services.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        services.AddSingleton(service, _ => func());
    }
}