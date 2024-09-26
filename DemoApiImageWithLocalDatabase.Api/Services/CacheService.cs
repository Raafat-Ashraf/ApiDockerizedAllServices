using DemoApiImageWithLocalDatabase.Api.Data;
using StackExchange.Redis;
using System.Text.Json;
using IDatabase = StackExchange.Redis.IDatabase;

namespace DemoApiImageWithLocalDatabase.Api.Services;

public class CacheService : ICacheService
{
    private IDatabase _cacheDb;
    public CacheService(ProductDbContext context)
    {
        var redis = ConnectionMultiplexer.Connect("3ec55ab94f3a_cache:6379");
        _cacheDb = redis.GetDatabase();
    }

    public T GetData<T>(string key)
    {
        var value = _cacheDb.StringGet(key);

        if (!string.IsNullOrEmpty(value))
            return JsonSerializer.Deserialize<T>(value!)!;

        return default;
    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);

        var isSet = _cacheDb.StringSet(key, JsonSerializer.Serialize<T>(value), expiryTime);

        return isSet;

    }

    public object RemoveData(string key)
    {
        var exist = _cacheDb.KeyExists(key);

        return exist && _cacheDb.KeyDelete(key);
    }
}
