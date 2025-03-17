using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
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
            if(context.HttpContext.Request.Headers["Authorization"].Count == 0)
            {
                context.Result = new UnauthorizedResult();
            }

            var token = context.HttpContext.Request.Headers["Authorization"].ToString();
            var claimsPrincipal = TokenService.ValidateToken(token);
            var role = claimsPrincipal.FindFirst(ClaimTypes.Role);
            //if(role.Value.Equals(""))


        }
    }
}
