using WebApplication3.Biz;
using WebApplication3.Models.DB;

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
                throw new Exception("用户未授权！");
            }

            var userBiz = new UserBiz();
            var user = userBiz.GetUserByCode(uCodeLong);
            if (user == null)
            {
                throw new Exception("用户不存在！");
            }

            return user;
        }
    }
}