using FreeRedis;

namespace WebApplication3.Foundation.Helper;

public class RedisHelper
{
    private static readonly RedisClient _redisClient;
    static string RedisIP = ConfigHelper.GetString("RedisIP");
    static string RedisPort = ConfigHelper.GetString("RedisPort");
    static string RedisPassword = ConfigHelper.GetString("RedisPassword");
    static RedisHelper()
    {
        // 从配置文件中读取 Redis 连接信息
        string redisConnectionString = $"{RedisIP}:{RedisPort},password={RedisPassword},defaultDatabase=0";

        // 初始化 RedisClient
        _redisClient = new RedisClient(redisConnectionString);
    }

    /// <summary>
    /// 设置键值对
    /// </summary>
    public static void Set(string key, string value, int expireSeconds = 0)
    {
        if (expireSeconds > 0)
        {
            _redisClient.Set(key, value, expireSeconds);
        }
        else
        {
            _redisClient.Set(key, value);
        }
    }

    /// <summary>
    /// 获取值
    /// </summary>
    public static string Get(string key)
    {
        return _redisClient.Get(key);
    }

    /// <summary>
    /// 删除键
    /// </summary>
    public static void Remove(string key)
    {
        _redisClient.Del(key);
    }

    /// <summary>
    /// 检查键是否存在
    /// </summary>
    public static bool Exists(string key)
    {
        return _redisClient.Exists(key);
    }

    /// <summary>
    /// 设置键的过期时间
    /// </summary>
    public static void Expire(string key, int expireSeconds)
    {
        _redisClient.Expire(key, expireSeconds);
    }
}