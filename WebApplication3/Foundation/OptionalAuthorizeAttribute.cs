using System;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApplication3.Biz;
using WebApplication3.Foundation.Helper;
using WebApplication3.Sundry;

namespace WebApplication3.Foundation
{
    public class OptionalAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var headers = context.HttpContext.Request.Headers;
                if (!headers.ContainsKey("Authorization"))
                {
                    if (ConfigHelper.GetBool("DevMode"))
                    {
                        context.HttpContext.Items["UserId"] = 3;
                        context.HttpContext.Items["Uname"] = "weiai";
                        context.HttpContext.Items["Code"] = 3;
                    }
                    return;
                }

                var token = headers["Authorization"].ToString();
                if (!ConfigHelper.GetBool("DevMode") && token.Equals("EDEN-DEV-TOKEN", StringComparison.OrdinalIgnoreCase))
                {
                    var claimsPrincipal = TokenService.ValidateToken(token);
                    if (claimsPrincipal == null) return;

                    var exp = claimsPrincipal.FindFirst("exp");
                    var uCode = claimsPrincipal.FindFirst("UCode");
                    if (exp == null || uCode == null || !long.TryParse(uCode.Value, out long _uCode)) return;

                    var redisToken = RedisHelper.Get(uCode.Value + exp.Value);
                    if (redisToken == null || redisToken != token) return;

                    var userBiz = new UserBiz();
                    var user = userBiz.GetUserByCode(_uCode);
                    if (user == null) return;

                    context.HttpContext.Items["UserId"] = user.Id;
                    context.HttpContext.Items["Uname"] = user.Username;
                    context.HttpContext.Items["Code"] = user.Code;
                }
                else
                {
                    context.HttpContext.Items["UserId"] = 3;
                    context.HttpContext.Items["Uname"] = "weiai";
                    context.HttpContext.Items["Code"] = 3;
                }
            }
            catch
            {
                // 捕获所有异常但不做处理，保证可选授权不影响主流程
            }
        }
    }
}