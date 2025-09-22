using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using TouchSocket.Core;
using WebApplication3.Biz;
using WebApplication3.Foundation.Exceptions;
using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;
using WebApplication3.Sundry;

namespace WebApplication3.Foundation
{
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        bool IsAdmin = false;

        public AuthorizeAttribute(bool IsAdmin)
        {
            this.IsAdmin = IsAdmin;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Request.Headers["Authorization"].Count == 0) throw new AuthenticationException("未获取到登录信息");

            var token = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (!ConfigHelper.GetBool("DevMode") && token.Equals("EDEN-DEV-TOKEN", StringComparison.OrdinalIgnoreCase))
            {

                var claimsPrincipal = TokenService.ValidateToken(token);
                if (claimsPrincipal == null) throw new AuthenticationException("鉴权失败！");

                var exp = claimsPrincipal.FindFirst("exp");
                var UCode = claimsPrincipal.FindFirst("UCode");
                if (exp == null || UCode == null) throw new AuthenticationException("鉴权失败！");

                var redisToken = RedisHelper.Get(UCode.Value + exp.Value);
                //redisToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjY2NjY2NjY2NiIsInJvbGUiOiIyIiwiUHVycG9zZSI6InJlZnJlc2hUb2tlbiIsIlVDb2RlIjoiMSIsIm5iZiI6MTc0NzE0NDcwMCwiZXhwIjoxNzQ5NzM2NzAwLCJpYXQiOjE3NDcxNDQ3MDAsImlzcyI6Iklzc3VlciIsImF1ZCI6IkF1ZGllbmNlIn0.5EJDhc_QJS88VOkSORIarwLqJ6ImQJIPufmUtT6w_II";
                if (redisToken == null || redisToken != token || !long.TryParse(UCode.Value, out long _uCode))
                    throw new AuthenticationException("鉴权失败！");

                UserBiz userBiz = new UserBiz();
                var user = userBiz.GetUserByCode(_uCode);

                // 将用户ID存储在HttpContext.Items中
                context.HttpContext.Items["UserId"] = user.Id;
                context.HttpContext.Items["Uname"] = user.Username;
                context.HttpContext.Items["Code"] = user.Code;

                if (user == null) throw new AuthenticationException("鉴权失败！");
                if (IsAdmin)
                    if (user.Role != 1)
                        throw new AuthenticationException("鉴权失败！");
            }
            else
            {
                // 开发模式下，允许使用固定的开发令牌
                context.HttpContext.Items["UserId"] = 3; // 假设开发者ID为1
                context.HttpContext.Items["Uname"] = "weiai";
                context.HttpContext.Items["Code"] = 3;
            }
        }
    }
}
