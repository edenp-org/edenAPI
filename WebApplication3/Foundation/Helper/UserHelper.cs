using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Biz;
using WebApplication3.Foundation.Exceptions;
using WebApplication3.Models.DB;
using WebApplication3.Sundry;

namespace WebApplication3.Foundation.Helper
{
    public static class UserHelper
    {
        public static User GetUserFromContext(HttpContext httpContext)
        {
            var userId = httpContext.Items["UserId"]?.ToString();
            var uname = httpContext.Items["Uname"]?.ToString();
            var uCode = httpContext.Items["Code"]?.ToString();

            if (string.IsNullOrEmpty(userId) || !long.TryParse(uCode, out long uCodeLong))
            {
                throw new CustomException("�û�δ��Ȩ��");
            }

            var userBiz = new UserBiz();
            var user = userBiz.GetUserByCode(uCodeLong);
            if (user == null)
            {
                throw new CustomException("�û������ڣ�");
            }

            return user;
        }
        public static User GetUserFromContextNoException(HttpContext httpContext)
        {
            var userId = httpContext.Items["UserId"]?.ToString();
            var uname = httpContext.Items["Uname"]?.ToString();
            var uCode = httpContext.Items["Code"]?.ToString();

            if (string.IsNullOrEmpty(userId) || !long.TryParse(uCode, out long uCodeLong)) return null;

            var userBiz = new UserBiz();
            var user = userBiz.GetUserByCode(uCodeLong);
            if (user == null) return null;

            return user;
        }
    }
}