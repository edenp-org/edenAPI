namespace WebApplication3.Foundation.Helper;

public static class DateTimeExtensions
{
    /// <summary>
    /// 将 DateTime 转换为 Unix 时间戳（秒）
    /// </summary>
    public static long ToUnixTimeSeconds(this DateTime dateTime)
    {
        return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 将 DateTime 转换为 Unix 时间戳（毫秒）
    /// </summary>
    public static long ToUnixTimeMilliseconds(this DateTime dateTime)
    {
        return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeMilliseconds();
    }
}