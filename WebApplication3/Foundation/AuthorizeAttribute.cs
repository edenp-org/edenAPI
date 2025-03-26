using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using WebApplication3.Biz;
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
            {
                context.Result = new UnauthorizedResult();
            }

            var token = context.HttpContext.Request.Headers["Authorization"].ToString();
            var claimsPrincipal = TokenService.ValidateToken(token);
            var role = claimsPrincipal.FindFirst(ClaimTypes.Name);


            UserBiz userBiz = new UserBiz();
            var user = userBiz.GetUserByUname(role.Value);

            // 将用户ID存储在HttpContext.Items中
            context.HttpContext.Items["UserId"] = user.Id;
            context.HttpContext.Items["Uname"] = user.Username;
            context.HttpContext.Items["Code"] = user.Code;
            UserTokenBiz tokenBiz = new UserTokenBiz();
            if (!tokenBiz.IsExist(role.Value, token, "登录")) throw new Exception("鉴权失败！");

            if (user == null) throw new Exception("鉴权失败！");
            if (IsAdmin) if (user.Role != 1) throw new Exception("鉴权失败！");
        }
    }
}
