using FreeSql.DataAnnotations;

namespace WebApplication3.Models.DB;

public class RequestLog
{
    [Column(IsIdentity = true, IsPrimary = true)]
    /// <summary>自增主键</summary>
    public long Id { get; set; }

    /// <summary>请求唯一ID（中间件生成并透传）</summary>
    public string RequestId { get; set; } = string.Empty;

    /// <summary>请求路径</summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>HTTP 方法</summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>查询字符串（? 后内容）</summary>
    public string QueryString { get; set; } = string.Empty;

    /// <summary>请求体（截断后保存）</summary>
    public string RequestBody { get; set; } = string.Empty;

    /// <summary>响应体（截断后保存）</summary>
    public string ResponseBody { get; set; } = string.Empty;

    /// <summary>HTTP 状态码</summary>
    public int StatusCode { get; set; }

    /// <summary>耗时（毫秒）</summary>
    public long ElapsedMs { get; set; }

    /// <summary>创建时间（UTC）</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}