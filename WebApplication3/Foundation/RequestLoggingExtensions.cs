using WebApplication3.Foundation.Middleware;
using WebApplication3.Middleware;

namespace WebApplication3.Foundation;

/// <summary>
/// 中间件扩展方法，便于链式调用
/// </summary>
public static class RequestLoggingExtensions
{
    /// <summary>
    /// 注册请求日志中间件
    /// </summary>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}