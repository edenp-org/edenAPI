namespace WebApplication3.Foundation.Helper;

public static class DateTimeExtensions
{
    /// <summary>
    /// �� DateTime ת��Ϊ Unix ʱ������룩
    /// </summary>
    public static long ToUnixTimeSeconds(this DateTime dateTime)
    {
        return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// �� DateTime ת��Ϊ Unix ʱ��������룩
    /// </summary>
    public static long ToUnixTimeMilliseconds(this DateTime dateTime)
    {
        return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeMilliseconds();
    }
}