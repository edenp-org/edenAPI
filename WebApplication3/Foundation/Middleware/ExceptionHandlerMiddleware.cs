using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        if (exception is CustomException customException)
        {
            result.Add("status", 400);
            result.Add("message", customException.Message);
        }
        else if(exception is AuthenticationException authenticationException)
        {
            result.Add("status", 400);
            result.Add("message", authenticationException.Message);
            context.Response.StatusCode = 401;
        }
        else
        {
            result.Add("status", 400);
            result.Add("message", "系统错误！");
            result.Add("errDate", timestampInMilliseconds);
            result.Add("hResult", exception.HResult);
        }

        NLogHelper.Error($"错误时间：{timestampInMilliseconds}错误代码:{exception.HResult}", exception);
        return context.Response.WriteAsJsonAsync(result);
    }
}