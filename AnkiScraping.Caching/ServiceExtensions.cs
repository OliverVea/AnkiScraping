using Microsoft.Extensions.DependencyInjection;
using NeoSmart.Caching.Sqlite;
using SQLitePCL;

namespace AnkiScraping.Caching;

public static class ServiceExtensions
{
    private const string SqliteFilePath = "I:/RiderProjects/AnkiScraping/AnkiScraping.CLI/cache.db";
    
    public static IServiceCollection AddSqliteIDistributedCache(this IServiceCollection services)
    {
        ISQLite3Provider provider = new SQLite3Provider_winsqlite3();
    
        raw.SetProvider(provider);
        services.AddSqliteCache(SqliteFilePath, provider);
        
        return services;
    }
}