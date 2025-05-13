using System.Net;
using WebApplication3.Foundation.Exceptions;
using WebApplication3.Foundation.Helper;

namespace WebApplication3.Foundation.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
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
        long timestampInMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (exception is CustomException ex)
        {
            result.Add("status", 400);
            result.Add("message", ex.Message);
        }
        else
        {
            result.Add("status", 400);
            result.Add("message", "ϵͳ����");
            result.Add("errDate", timestampInMilliseconds);
            result.Add("hResult", exception.HResult);
        }

        NLogHelper.Error($"����ʱ�䣺{timestampInMilliseconds}�������:{exception.HResult}", exception);
        return context.Response.WriteAsJsonAsync(result);
    }
}