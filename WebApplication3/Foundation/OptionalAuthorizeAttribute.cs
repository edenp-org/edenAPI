using System;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApplication3.Biz;
using WebApplication3.Foundation.Helper;
using WebApplication3.Sundry;

namespace WebApplication3.Foundation;


public class OptionalAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        try
        {
            if (context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                var token = context.HttpContext.Request.Headers["Authorization"].ToString();
                var claimsPrincipal = TokenService.ValidateToken(token);

                if (claimsPrincipal != null)
                {
                    var uCodeClaim = claimsPrincipal.FindFirst("UCode");
                    if (uCodeClaim != null && long.TryParse(uCodeClaim.Value, out long uCode))
                    {
                        var userBiz = new UserBiz();
                        var user = userBiz.GetUserByCode(uCode);

                        if (user != null)
                        {
                            // 将用户信息存储在 HttpContext.Items 中
                            context.HttpContext.Items["UserId"] = user.Id;
                            context.HttpContext.Items["Uname"] = user.Username;
                            context.HttpContext.Items["Code"] = user.Code;
                        }
                    }
                }
            }
        }
        catch
        {
            // 忽略异常，不强制身份认证
        }
    }
}