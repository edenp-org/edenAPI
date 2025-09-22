using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using WebApplication3.Foundation.Helper;

namespace WebApplication3.Middleware
{
    /// <summary>
    /// 拦截 /Work/{author}/{workCode}.TXT 请求，记录浏览量（Redis）
    /// </summary>
    public class WorkViewCountRedisMiddleware
    {
        private readonly RequestDelegate _next;
        private const int DedupExpireSeconds = 300; // 5分钟去重窗口

        public WorkViewCountRedisMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            if (!string.IsNullOrEmpty(path)
                && path.StartsWith("/Work/", StringComparison.OrdinalIgnoreCase)
                && path.EndsWith(".TXT", StringComparison.OrdinalIgnoreCase))
            {
                // /Work/{author}/{workCode}.TXT
                var seg = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (seg.Length == 3)
                {
                    var fileName = seg[2];
                    var codeStr = Path.GetFileNameWithoutExtension(fileName);
                    if (long.TryParse(codeStr, out long workCode))
                    {
                        // 用户标识：优先用户 Code，其次 IP
                        var userCodeObj = context.Items.ContainsKey("Code") ? context.Items["Code"] : null;
                        string uid = userCodeObj?.ToString();
                        if (string.IsNullOrEmpty(uid))
                        {
                            uid = context.Connection.RemoteIpAddress?.ToString() ?? "0";
                        }

                        var dedupKey = $"work:view:dedup:{workCode}:{uid}";
                        if (!RedisHelper.Exists(dedupKey))
                        {
                            // 计数 key
                            var counterKey = $"work:view:{workCode}";
                            RedisHelper.Incr(counterKey, 1);
                            RedisHelper.Set(dedupKey, "1", DedupExpireSeconds);
                        }
                    }
                }
            }

            await _next(context);
        }
    }

    public static class WorkViewCountRedisMiddlewareExtensions
    {
        public static IApplicationBuilder UseWorkViewCountRedis(this IApplicationBuilder app)
        {
            return app.UseMiddleware<WorkViewCountRedisMiddleware>();
        }
    }
}