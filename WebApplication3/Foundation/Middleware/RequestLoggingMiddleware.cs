using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;

namespace WebApplication3.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private const int MaxBodyLength = 10240;

        public RequestLoggingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString("N");
            context.Items["RequestId"] = requestId;
            context.Response.Headers["X-Request-ID"] = requestId;

            var requestBody = await ReadRequestBodyAsync(context.Request);

            var originalBody = context.Response.Body;
            await using var mem = new MemoryStream();
            context.Response.Body = mem;

            await _next(context);

            var responseBody = await ReadResponseBodyAsync(mem);
            sw.Stop();

            var log = new RequestLog
            {
                RequestId = requestId,
                Path = context.Request.Path,
                Method = context.Request.Method,
                QueryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value ?? "" : "",
                RequestBody = Truncate(requestBody),
                ResponseBody = Truncate(responseBody),
                StatusCode = context.Response.StatusCode,
                ElapsedMs = sw.ElapsedMilliseconds
            };

            try
            {
                await LoggingFreeSqlHelper.Instance.Insert(log).ExecuteAffrowsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("写请求日志(SQLite)失败: " + ex.Message);
            }

            mem.Position = 0;
            await mem.CopyToAsync(originalBody);
            context.Response.Body = originalBody;
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            if (request.ContentLength == null || request.ContentLength == 0) return "";
            request.EnableBuffering();
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }

        private static async Task<string> ReadResponseBodyAsync(MemoryStream ms)
        {
            ms.Position = 0;
            using var reader = new StreamReader(ms, Encoding.UTF8, leaveOpen: true);
            return await reader.ReadToEndAsync();
        }

        private static string Truncate(string s) =>
            string.IsNullOrEmpty(s) ? "" :
            (s.Length > MaxBodyLength ? s.Substring(0, MaxBodyLength) + "...(truncated)" : s);
    }
}