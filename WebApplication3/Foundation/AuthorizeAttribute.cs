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
            if (context.HttpContext.Request.Headers["Authorization"].Count == 0)
                throw new AuthenticationException("未获取到登录信息");

            var token = context.HttpContext.Request.Headers["Authorization"].ToString();
            var devMode = ConfigHelper.GetBool("DevMode");
            const string DevToken = "EDEN-DEV-TOKEN";

            // 开发模式且使用固定开发令牌：直接注入开发用户
            if (devMode && token.Equals(DevToken, StringComparison.OrdinalIgnoreCase))
            {
                context.HttpContext.Items["UserId"] = 3;
                context.HttpContext.Items["Uname"] = "weiai";
                context.HttpContext.Items["Code"] = 3;
                return;
            }

            // 非开发模式禁止使用开发令牌
            if (!devMode && token.Equals(DevToken, StringComparison.OrdinalIgnoreCase))
                throw new AuthenticationException("非法开发令牌");

            // 下面走正常 token 校验
            var claimsPrincipal = TokenService.ValidateToken(token);
            if (claimsPrincipal == null) throw new AuthenticationException("鉴权失败！");

            var exp = claimsPrincipal.FindFirst("exp");
            var UCode = claimsPrincipal.FindFirst("UCode");
            if (exp == null || UCode == null) throw new AuthenticationException("鉴权失败！");

            var redisToken = RedisHelper.Get(UCode.Value + exp.Value);
            if (redisToken == null || redisToken != token || !long.TryParse(UCode.Value, out long _uCode))
                throw new AuthenticationException("鉴权失败！");

            UserBiz userBiz = new UserBiz();
            var user = userBiz.GetUserByCode(_uCode);
            if (user == null) throw new AuthenticationException("鉴权失败！");

            // 将用户信息存储在 HttpContext.Items 中
            context.HttpContext.Items["UserId"] = user.Id;
            context.HttpContext.Items["Uname"] = user.Username;
            context.HttpContext.Items["Code"] = user.Code;

            if (IsAdmin && user.Role != 1)
                throw new AuthenticationException("鉴权失败！");
        }
    }
}
