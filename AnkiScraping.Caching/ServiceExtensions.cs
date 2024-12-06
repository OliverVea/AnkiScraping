using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeoSmart.Caching.Sqlite;
using SQLitePCL;

namespace AnkiScraping.Caching;

public static class ServiceExtensions
{
    private const string ConfigurationKey = "Cache";
    private const string DefaultSqliteFilePath = "./cache.db";
    
    public static IServiceCollection AddSqliteIDistributedCache(this IServiceCollection services, IConfiguration configuration)
    {
        ISQLite3Provider provider = new SQLite3Provider_winsqlite3();
        
        var sqliteFilePath = configuration[ConfigurationKey] ?? DefaultSqliteFilePath;
    
        raw.SetProvider(provider);
        services.AddSqliteCache(sqliteFilePath, provider);
        
        return services;
    }
}