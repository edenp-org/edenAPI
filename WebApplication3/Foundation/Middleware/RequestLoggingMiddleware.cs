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
            var mem = new MemoryStream();
            context.Response.Body = mem;

            Exception? pipelineException = null;
            string responseBody = string.Empty;

            try
            {
                await _next(context);
                // normal path: capture response body for logging
                responseBody = await ReadResponseBodyAsync(mem);
                mem.Position = 0;
                await mem.CopyToAsync(originalBody);
            }
            catch (Exception ex)
            {
                // exception path: read whatever was written so far, but DO NOT copy to response
                pipelineException = ex;
                try
                {
                    mem.Position = 0;
                    responseBody = await ReadResponseBodyAsync(mem);
                }
                catch
                {
                    // ignore logging read errors
                }
                throw;
            }
            finally
            {
                sw.Stop();

                var log = new RequestLog
                {
                    RequestId = requestId,
                    Path = context.Request.Path,
                    Method = context.Request.Method,
                    QueryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value ?? string.Empty : string.Empty,
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

                // Always restore the original body stream so upstream middleware can write (e.g., exception handler)
                context.Response.Body = originalBody;
                await mem.DisposeAsync();
            }
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