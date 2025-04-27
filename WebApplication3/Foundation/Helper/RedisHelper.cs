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
        // �������ļ��ж�ȡ Redis ������Ϣ
        string redisConnectionString = $"{RedisIP}:{RedisPort},password={RedisPassword},defaultDatabase=0";

        // ��ʼ�� RedisClient
        _redisClient = new RedisClient(redisConnectionString);
    }

    /// <summary>
    /// ���ü�ֵ��
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
    /// ��ȡֵ
    /// </summary>
    public static string Get(string key)
    {
        return _redisClient.Get(key);
    }

    /// <summary>
    /// ɾ����
    /// </summary>
    public static void Remove(string key)
    {
        _redisClient.Del(key);
    }

    /// <summary>
    /// �����Ƿ����
    /// </summary>
    public static bool Exists(string key)
    {
        return _redisClient.Exists(key);
    }

    /// <summary>
    /// ���ü��Ĺ���ʱ��
    /// </summary>
    public static void Expire(string key, int expireSeconds)
    {
        _redisClient.Expire(key, expireSeconds);
    }
}