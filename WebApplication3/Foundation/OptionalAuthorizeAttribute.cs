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
                var devMode = ConfigHelper.GetBool("DevMode");
                const string DevToken = "EDEN-DEV-TOKEN";

                // 无授权头：开发模式下给固定账号；否则忽略
                if (!headers.ContainsKey("Authorization"))
                {
                    if (devMode)
                    {
                        context.HttpContext.Items["UserId"] = 3;
                        context.HttpContext.Items["Uname"] = "weiai";
                        context.HttpContext.Items["Code"] = 3;
                    }
                    return;
                }

                var token = headers["Authorization"].ToString();

                // 开发模式 + 开发令牌：直接注入固定账号
                if (devMode && token.Equals(DevToken, StringComparison.OrdinalIgnoreCase))
                {
                    context.HttpContext.Items["UserId"] = 3;
                    context.HttpContext.Items["Uname"] = "weiai";
                    context.HttpContext.Items["Code"] = 3;
                    return;
                }

                // 非开发模式下使用开发令牌：忽略，不报错（可选授权）
                if (!devMode && token.Equals(DevToken, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                // 正常 JWT 校验流程
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
            catch
            {
                // 可选授权：吞掉异常，保持主流程正常
            }
        }
    }
}