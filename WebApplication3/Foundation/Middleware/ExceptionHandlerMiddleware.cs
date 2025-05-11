using System.Net;
using WebApplication3.Foundation.Exceptions;
using WebApplication3.Foundation.Helper;

namespace WebApplication3.Foundation.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        Dictionary<string, object> result = new Dictionary<string, object>();

        if (exception is CustomException ex)
        {
            result.Add("status", 400);
            result.Add("message", ex.Message);
        }
        else
        {
            long timestampInMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            result.Add("status", 400);
            result.Add("message", "系统错误！");
            result.Add("errDate", timestampInMilliseconds);
            result.Add("hResult", exception.HResult);
            NLogHelper.Error($"错误时间：{timestampInMilliseconds}错误代码:{exception.HResult}", exception);
        }

        return context.Response.WriteAsJsonAsync(result);
    }
}