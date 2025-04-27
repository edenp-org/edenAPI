using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using TouchSocket.Core;
using WebApplication3.Biz;
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
            try
            {


                if (context.HttpContext.Request.Headers["Authorization"].Count == 0)
                {
                    context.Result = new JsonResult(new
                    {
                        status = 400,
                        message = new { message = "未获取到登录信息！" }.ToJsonString(),
                    });
                    context.HttpContext.Response.StatusCode = 401;
                    return;
                }

                var token = context.HttpContext.Request.Headers["Authorization"].ToString();

                var claimsPrincipal = TokenService.ValidateToken(token);
                if (claimsPrincipal == null) throw new Exception("鉴权失败！");

                var exp = claimsPrincipal.FindFirst("exp");
                var UCode = claimsPrincipal.FindFirst("UCode");
                if (exp == null || UCode == null) throw new Exception("鉴权失败！");

                var redisToken = RedisHelper.Get(UCode.Value + exp.Value);
                if (redisToken == null || redisToken != token || !long.TryParse(UCode.Value, out long _uCode)) throw new Exception("鉴权失败！");

                UserBiz userBiz = new UserBiz();
                var user = userBiz.GetUserByCode(_uCode);

                // 将用户ID存储在HttpContext.Items中
                context.HttpContext.Items["UserId"] = user.Id;
                context.HttpContext.Items["Uname"] = user.Username;
                context.HttpContext.Items["Code"] = user.Code;

                if (user == null) throw new Exception("鉴权失败！");
                if (IsAdmin)
                    if (user.Role != 1)
                        throw new Exception("鉴权失败！");
            }
            catch (Exception e)
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new JsonResult(new
                {
                    status = 400,
                    message = $"鉴权失败！错误代码：0x{e.HResult:X}"
                });
            }
        }
    }
}
