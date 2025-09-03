using System.Security.Claims;
using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using WebApplication3.Biz;
using WebApplication3.Foundation;
using WebApplication3.Foundation.Exceptions;
using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;
using WebApplication3.Sundry;

namespace WebApplication3.Controllers
{
    /// <summary>
    /// 用户相关接口控制器。
    /// </summary>
    [Route("User")]
    public class UserController(ICaptcha captcha) : Controller
    {
        #region 入参类型

        /// <summary>
        /// 登录请求最外层包装。
        /// </summary>
        public class LoginRequest
        {
            /// <summary>
            /// 实际请求数据。
            /// </summary>
            public LoginRequestData data { get; set; }

            /// <summary>
            /// 登录请求参数（用户名与邮箱二选一）。
            /// </summary>
            public class LoginRequestData
            {
                /// <summary>密码（前端明文，后端拼接盐后哈希）。</summary>
                public string Password { get; set; }

                /// <summary>图形验证码 ID。</summary>
                public string CaptchaId { get; set; }

                /// <summary>图形验证码输入内容。</summary>
                public string CaptchaInput { get; set; }

                /// <summary>用户名（用户名登录时填写）。</summary>
                public string Uname { get; set; }

                /// <summary>邮箱（邮箱登录时填写）。</summary>
                public string Email { get; set; }
            }
        }

        /// <summary>
        /// 发送邮箱验证码请求。
        /// </summary>
        public class SendEmailVerificationCodeRequest
        {
            /// <summary>
            /// 请求数据。
            /// </summary>
            public SendEmailVerificationCodeRequestData data { get; set; }

            /// <summary>
            /// 发送邮箱验证码参数。
            /// </summary>
            public class SendEmailVerificationCodeRequestData
            {
                /// <summary>目标邮箱。</summary>
                public string Email { get; set; }

                /// <summary>图形验证码 ID。</summary>
                public string CaptchaId { get; set; }

                /// <summary>图形验证码输入。</summary>
                public string CaptchaInput { get; set; }
            }
        }

        /// <summary>
        /// 注册请求。
        /// </summary>
        public class RegisterRequest
        {
            /// <summary>
            /// 请求数据。
            /// </summary>
            public RegisterRequestData data { get; set; }

            /// <summary>
            /// 注册参数。
            /// </summary>
            public class RegisterRequestData
            {
                /// <summary>邮箱。</summary>
                public string Email { get; set; }

                /// <summary>用户名。</summary>
                public string Uname { get; set; }

                /// <summary>密码。</summary>
                public string Password { get; set; }

                /// <summary>确认密码。</summary>
                public string PasswordSecond { get; set; }

                /// <summary>邮箱验证码。</summary>
                public string Captcha { get; set; }
            }
        }

        /// <summary>
        /// 添加喜欢标签请求。
        /// </summary>
        public class AddUsersLikeTagRequest
        {
            /// <summary>
            /// 请求数据。
            /// </summary>
            public AddUsersLikeTagRequestData data { get; set; }

            /// <summary>
            /// 添加喜欢标签参数。
            /// </summary>
            public class AddUsersLikeTagRequestData
            {
                /// <summary>标签唯一编码。</summary>
                public long TagCode { get; set; }
            }
        }

        /// <summary>
        /// 删除喜欢标签请求。
        /// </summary>
        public class DeleteUsersLikeTagRequest
        {
            /// <summary>
            /// 请求数据。
            /// </summary>
            public DeleteUsersLikeTagRequestRequestData data { get; set; }

            /// <summary>
            /// 删除喜欢标签参数。
            /// </summary>
            public class DeleteUsersLikeTagRequestRequestData
            {
                /// <summary>标签唯一编码。</summary>
                public long TagCode { get; set; }
            }
        }

        /// <summary>
        /// 添加不喜欢标签请求。
        /// </summary>
        public class AddUserDislikedTagRequest
        {
            /// <summary>
            /// 请求数据。
            /// </summary>
            public AddUserDislikedTagRequestData data { get; set; }

            /// <summary>
            /// 添加不喜欢标签参数。
            /// </summary>
            public class AddUserDislikedTagRequestData
            {
                /// <summary>标签唯一编码。</summary>
                public long TagCode { get; set; }
            }
        }

        /// <summary>
        /// 删除不喜欢标签请求。
        /// </summary>
        public class DeleteUserDislikedTagRequest
        {
            /// <summary>
            /// 请求数据。
            /// </summary>
            public DeleteUserDislikedTagRequestData data { get; set; }

            /// <summary>
            /// 删除不喜欢标签参数。
            /// </summary>
            public class DeleteUserDislikedTagRequestData
            {
                /// <summary>标签唯一编码。</summary>
                public long TagCode { get; set; }
            }
        }

        /// <summary>
        /// 添加喜欢的作品请求。
        /// </summary>
        public class AddUserLikeWorkRequest
        {
            /// <summary>
            /// 请求数据。
            /// </summary>
            public AddUserLikeWorkRequestData data { get; set; }

            /// <summary>
            /// 添加喜欢作品参数。
            /// </summary>
            public class AddUserLikeWorkRequestData
            {
                /// <summary>作品编码。</summary>
                public long WorkCode { get; set; }
            }
        }

        /// <summary>
        /// 找回或修改密码请求。
        /// </summary>
        public class RetrievePasswordRequest
        {
            /// <summary>
            /// 请求数据。
            /// </summary>
            public RetrievePasswordRequestData data { get; set; }

            /// <summary>
            /// 找回或修改密码参数。
            /// </summary>
            public class RetrievePasswordRequestData
            {
                /// <summary>邮箱。</summary>
                public string Email { get; set; }

                /// <summary>旧密码（已登录修改时必须）。</summary>
                public string OldPassword { get; set; }

                /// <summary>新密码。</summary>
                public string Password { get; set; }

                /// <summary>确认新密码。</summary>
                public string PasswordSecond { get; set; }

                /// <summary>邮箱验证码。</summary>
                public string Captcha { get; set; }
            }
        }

        /// <summary>
        /// 更新头像请求（预留结构）。
        /// </summary>
        public class UpdateAvatarByFileRequest
        {
            /// <summary>
            /// 请求数据。
            /// </summary>
            public RetrievePasswordRequestData data { get; set; }

            /// <summary>
            /// 预留字段（当前未使用）。
            /// </summary>
            public class RetrievePasswordRequestData
            {
                /// <summary>作品编码（未使用）。</summary>
                public long WorkCode { get; set; }
            }
        }
        #endregion

        /// <summary>
        /// 生成用户编码时的并发锁，避免并发重复。
        /// </summary>
        private static readonly object _codeLock = new object();

        /// <summary>
        /// 获取用户列表（非管理员隐藏邮箱及审核信息）。
        /// </summary>
        /// <param name="username">按用户名过滤。</param>
        /// <param name="email">按邮箱过滤（仅管理员可用）。</param>
        /// <param name="ucode">按用户编码过滤。</param>
        /// <returns>用户集合。</returns>
        [OptionalAuthorize(), HttpGet("GetUsers")]
        public Dictionary<string, object> GetUsers(string username = "", string email = "", long ucode = 0)
        {
            User user = UserHelper.GetUserFromContextNoException(HttpContext);
            UserBiz userBiz = new UserBiz();
            var users = userBiz.GetUsers(username, user?.Role == 1 ? email : "", ucode)
                .Select(u => new
                {
                    u.Code,
                    u.Username,
                    u.Role,
                    Email = user?.Role == 1 ? u.Email : null,
                    ExamineCount = user?.Role == 1 ? u.ExamineCount : (int?)null,
                    LastExamineTime = user?.Role == 1 ? u.LastExamineTime : (DateTime?)null
                }).ToList();
            return new Dictionary<string, object>()
            {
                { "code", 200 },
                { "data", users }
            };
        }

        /// <summary>
        /// 用户登录（支持用户名或邮箱）。
        /// </summary>
        /// <param name="request">登录参数。</param>
        /// <returns>访问令牌。</returns>
        [HttpPost("Login")]
        public Dictionary<string, object> Login([FromBody] LoginRequest request)
        {
            var dic = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(request.data.Password)) throw new CustomException("请输入密码！");
            if (string.IsNullOrEmpty(request.data.CaptchaId)) throw new CustomException("未获取到验证码ID！");
            if (string.IsNullOrEmpty(request.data.CaptchaInput)) throw new CustomException("请输入验证码！");
            if (string.IsNullOrEmpty(request.data.Uname) && string.IsNullOrEmpty(request.data.Email)) throw new CustomException("请选择登录方式");
            if (!string.IsNullOrEmpty(request.data.Uname) && !string.IsNullOrEmpty(request.data.Email)) throw new CustomException("请选择登录方式");

            if (!ConfigHelper.GetBool("DevMode"))
            {
                if (!captcha.Validate(request.data.CaptchaId, request.data.CaptchaInput)) throw new CustomException("验证码错误！");
            }

            var userBiz = new UserBiz();
            var userTokenBiz = new UserTokenBiz();

            var user = !string.IsNullOrEmpty(request.data.Email)
                ? userBiz.GetUserByEmail(request.data.Email)
                : userBiz.GetUserByUname(request.data.Uname);
            if (user == null) throw new CustomException("用户名或邮箱或密码不存在！");

            if (!user.Password.Equals(EncryptionHelper.ComputeSHA256(request.data.Password + user.Confuse)))
                throw new CustomException("用户名或邮箱或密码不存在！");

            int expirationHours = ConfigHelper.GetInt("TokenExpirationHours");
            var refreshTokenExpires = DateTime.UtcNow.AddHours(expirationHours);
            var accessTokenExpires = DateTime.UtcNow.AddHours(1);

            var refreshToken = TokenService.GenerateToken(user.Username, user.Role.ToString(), "refreshToken", user.Code, refreshTokenExpires);
            var accessToken = TokenService.GenerateToken(user.Username, user.Role.ToString(), "accessToken", user.Code, accessTokenExpires);

            userTokenBiz.Add(new UserToken
            {
                Expiration = refreshTokenExpires,
                CreatedAt = DateTime.UtcNow,
                Purpose = "refreshToken",
                Token = refreshToken,
                Username = user.Username
            });

            var accessTokenExpiresTimeSeconds = accessTokenExpires.ToUnixTimeSeconds();
            RedisHelper.Set(user.Code + accessTokenExpiresTimeSeconds.ToString(), accessToken, (int)(accessTokenExpires - DateTime.UtcNow).TotalSeconds);

            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMonths(1)
            });

            dic.Add("status", 200);
            dic.Add("message", "登录成功！");
            dic.Add("data", accessToken);
            return dic;
        }

        /// <summary>
        /// 找回或修改密码（登录用户需旧密码，未登录需邮箱验证码）。
        /// </summary>
        /// <param name="request">参数。</param>
        /// <returns>操作结果。</returns>
        [OptionalAuthorize(), HttpPost("RetrievePassword")]
        public Dictionary<string, object> RetrievePassword([FromBody] RetrievePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.data.Email)) throw new CustomException("请输入邮箱");
            if (string.IsNullOrEmpty(request.data.Password)) throw new CustomException("请输入新密码");
            if (string.IsNullOrEmpty(request.data.PasswordSecond)) throw new CustomException("请输入新密码的确认");
            if (string.IsNullOrEmpty(request.data.Captcha)) throw new CustomException("请输入验证码");

            var user = UserHelper.GetUserFromContextNoException(HttpContext);
            UserTokenBiz userTokenBiz = new UserTokenBiz();
            UserBiz userBiz = new UserBiz();

            if (user == null)
            {
                if (request.data.Password != request.data.PasswordSecond) throw new CustomException("两次密码不一致！");
                var userToken = userTokenBiz.GetTokenByUserAndPurpose(request.data.Email, "邮箱")
                    .FirstOrDefault(u => u.Token.Equals(request.data.Captcha));
                if (userToken == null) throw new CustomException("验证码错误或已过期！");
                user = userBiz.GetUserByEmail(request.data.Email) ?? throw new CustomException("邮箱未注册！");
            }
            else
            {
                if (string.IsNullOrEmpty(request.data.OldPassword)) throw new Exception("旧密码不能为空！");
                if (EncryptionHelper.ComputeSHA256(request.data.OldPassword + user.Confuse) != user.Password)
                    throw new CustomException("旧密码错误！");
            }

            string password = EncryptionHelper.ComputeSHA256(request.data.Password + user.Confuse);
            userBiz.RetrievePassword(user.Code, password);

            return new Dictionary<string, object>()
            {
                { "status", 200 },
                { "message", "成功" }
            };
        }

        /// <summary>
        /// 刷新访问令牌。
        /// </summary>
        /// <returns>新的访问令牌。</returns>
        [HttpPost("RefreshToken")]
        public Dictionary<string, object> RefreshToken()
        {
            var dic = new Dictionary<string, object>();

            if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                throw new CustomException("未提供刷新令牌！");

            var claimsPrincipal = TokenService.ValidateToken(refreshToken);
            if (claimsPrincipal == null) throw new CustomException("鉴权失败！");

            var exp = claimsPrincipal.FindFirst(ClaimTypes.Name);
            var UCode = claimsPrincipal.FindFirst("UCode");
            if (exp == null || UCode == null || !long.TryParse(UCode.Value, out long _result))
                throw new CustomException("鉴权失败！");

            var userTokenBiz = new UserTokenBiz();
            var userToken = userTokenBiz.GetTokenByUserAndPurpose(exp.Value, "refreshToken");
            if (userToken == null || userToken.All(e => e.Expiration < DateTime.UtcNow))
                throw new CustomException("刷新令牌无效或已过期！");

            var userBiz = new UserBiz();
            var user = userBiz.GetUserByCode(_result) ?? throw new CustomException("用户不存在！");

            var accessTokenExpires = DateTime.UtcNow.AddHours(1);
            var accessToken = TokenService.GenerateToken(user.Username, user.Role.ToString(), "accessToken", user.Code, accessTokenExpires);
            RedisHelper.Set(user.Code + accessTokenExpires.ToUnixTimeSeconds().ToString(), accessToken, (int)(accessTokenExpires - DateTime.UtcNow).TotalSeconds);

            dic.Add("message", "令牌刷新成功！");
            dic.Add("data", accessToken);
            return dic;
        }

        /// <summary>
        /// 发送邮箱验证码（注册 / 找回密码）。
        /// </summary>
        /// <param name="request">请求参数。</param>
        /// <returns>操作结果。</returns>
        [HttpPost("SendEmailVerificationCode")]
        public Dictionary<string, object> SendEmailVerificationCode([FromBody] SendEmailVerificationCodeRequest request)
        {
            var dic = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(request.data.Email)) throw new CustomException("请输入邮箱！");
            if (string.IsNullOrEmpty(request.data.CaptchaId)) throw new CustomException("请输入验证码ID！");
            if (string.IsNullOrEmpty(request.data.CaptchaInput)) throw new CustomException("请输入验证码！");
            if (!ConfigHelper.GetBool("DevMode"))
            {
                if (!captcha.Validate(request.data.CaptchaId, request.data.CaptchaInput)) throw new CustomException("错误的验证码");
            }

            var token = new Random().Next(100000, 999999).ToString();

            var userTokenBiz = new UserTokenBiz();
            userTokenBiz.Add(new UserToken
            {
                Expiration = DateTime.UtcNow.AddMinutes(5),
                CreatedAt = DateTime.UtcNow,
                Purpose = "邮箱",
                Token = token,
                Username = request.data.Email
            });

            EmailHelper.SendEmail(request.data.Email, "邮箱验证", $"您的验证码是：{token} \r\n 该验证码5分钟后过期!");

            dic.Add("status", 200);
            dic.Add("message", "验证码已发送至邮箱，请查收！");
            return dic;
        }

        /// <summary>
        /// 用户注册。
        /// </summary>
        /// <param name="request">注册参数。</param>
        /// <returns>操作结果。</returns>
        [HttpPost("Register")]
        public Dictionary<string, object> Register([FromBody] RegisterRequest request)
        {
            var dic = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(request.data.Email)) throw new CustomException("请输入邮箱！");
            if (string.IsNullOrEmpty(request.data.Uname)) throw new CustomException("请输入用户名！");
            if (string.IsNullOrEmpty(request.data.Password)) throw new CustomException("请输入密码！");
            if (string.IsNullOrEmpty(request.data.PasswordSecond)) throw new CustomException("请输入确认密码！");
            if (string.IsNullOrEmpty(request.data.Captcha)) throw new CustomException("请输入验证码！");
            if (request.data.Password != request.data.PasswordSecond) throw new CustomException("两次密码不一致！");
            if (string.IsNullOrEmpty(request.data.Email) || string.IsNullOrEmpty(request.data.Uname) || string.IsNullOrEmpty(request.data.Password))
                throw new CustomException("请填写完整信息！");

            var userBiz = new UserBiz();
            var userTokenBiz = new UserTokenBiz();

            if (userBiz.GetUserByEmail(request.data.Email) != null) throw new CustomException("邮箱已被注册！");
            if (userBiz.GetUserByUname(request.data.Uname) != null) throw new CustomException("用户名已被注册！");

            var userToken = userTokenBiz.GetTokenByUserAndPurpose(request.data.Email, "邮箱")
                .FirstOrDefault(u => u.Token.Equals(request.data.Captcha));
            if (userToken == null) throw new CustomException("验证码错误或已过期！");

            lock (_codeLock)
            {
                var newCodeUser = userBiz.GetNewCode();
                var confuse = new Random().Next(100000, 999999).ToString("X");
                userBiz.Add(new User
                {
                    Email = request.data.Email,
                    Username = request.data.Uname,
                    Password = EncryptionHelper.ComputeSHA256(request.data.Password + confuse),
                    Code = newCodeUser,
                    Confuse = confuse,
                    Role = 2
                });
            }

            dic.Add("status", 200);
            dic.Add("message", "注册成功！");
            return dic;
        }

        /// <summary>
        /// 添加喜欢的标签。
        /// </summary>
        /// <param name="request">标签参数。</param>
        /// <returns>操作结果。</returns>
        [Authorize(false), HttpPost("AddUsersLikeTag")]
        public Dictionary<string, object> AddUsersLikeTag([FromBody] AddUsersLikeTagRequest request)
        {
            var dic = new Dictionary<string, object>();
            var user = UserHelper.GetUserFromContext(HttpContext);
            var tag = new TagBiz().GetTagByCode(request.data.TagCode) ?? throw new CustomException("标签不存在！");

            UserBiz userBiz = new UserBiz();
            var favoriteTags = userBiz.GetUserFavoriteTagByUserId(user.Code, tag.Id);
            if (favoriteTags != null) throw new CustomException("已添加该标签！");
            userBiz.AddUserFavoriteTag(new UserFavoriteTag
            {
                TagCode = tag.Code,
                UserCode = user.Code,
                TagName = tag.Name,
                UserName = user.Username
            });

            dic.Add("status", 200);
            dic.Add("message", "成功");
            return dic;
        }

        /// <summary>
        /// 删除喜欢的标签。
        /// </summary>
        /// <param name="request">标签参数。</param>
        /// <returns>操作结果。</returns>
        [Authorize(false), HttpPost("DeleteUsersLikeTag")]
        public Dictionary<string, object> DeleteUsersLikeTag([FromBody] DeleteUsersLikeTagRequest request)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var user = UserHelper.GetUserFromContext(HttpContext);
            UserBiz userBiz = new UserBiz();
            userBiz.DeleteUsersLikeTag(user.Code, request.data.TagCode);
            dic.Add("status", "200");
            dic.Add("message", "成功");
            return dic;
        }

        /// <summary>
        /// 获取当前用户喜欢的标签列表。
        /// </summary>
        /// <returns>喜欢的标签集合。</returns>
        [Authorize(false), HttpGet("GetUserUsersLikeTag")]
        public Dictionary<string, object> GetUserUsersLikeTag()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var user = UserHelper.GetUserFromContext(HttpContext);
            UserBiz userBiz = new UserBiz();
            var list = userBiz.GetUserFavoriteTagByUserId(user.Code);
            dic.Add("status", "200");
            dic.Add("message", "成功");
            dic.Add("data", list.Select(i => new { i.UserCode, i.TagCode, i.TagName }).ToList());
            return dic;
        }

        /// <summary>
        /// 添加不喜欢的标签。
        /// </summary>
        /// <param name="request">标签参数。</param>
        /// <returns>操作结果。</returns>
        [Authorize(false), HttpPost("AddUserDislikedTag")]
        public Dictionary<string, object> AddUserDislikedTag([FromBody] AddUserDislikedTagRequest request)
        {
            var dic = new Dictionary<string, object>();
            var user = UserHelper.GetUserFromContext(HttpContext);
            var tag = new TagBiz().GetTagByCode(request.data.TagCode) ?? throw new CustomException("标签不存在！");
            UserBiz userBiz = new UserBiz();
            var userDislikedTag = userBiz.GetUserDislikedTagByUserId(user.Code, tag.Id);
            if (userDislikedTag != null) throw new CustomException("已添加该标签！");
            userBiz.AddUserDislikedTag(new UserDislikedTag
            {
                TagCode = request.data.TagCode,
                UserCode = user.Code,
                TagName = tag.Name,
                UserName = user.Username
            });
            dic.Add("status", 200);
            dic.Add("message", "成功");
            return dic;
        }

        /// <summary>
        /// 获取不喜欢的标签列表。
        /// </summary>
        /// <returns>不喜欢的标签集合。</returns>
        [Authorize(false), HttpGet("GetUserDislikedTag")]
        public Dictionary<string, object> GetUserDislikedTag()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var user = UserHelper.GetUserFromContext(HttpContext);
            UserBiz userBiz = new UserBiz();
            var list = userBiz.GetUserDislikedTagByUserId(user.Code);
            dic.Add("status", 200);
            dic.Add("message", "成功");
            dic.Add("data", list.Select(i => new { i.TagName, i.UserCode }));
            return dic;
        }

        /// <summary>
        /// 删除不喜欢的标签。
        /// </summary>
        /// <param name="request">标签参数。</param>
        /// <returns>操作结果。</returns>
        [Authorize(false), HttpPost("DeleteUserDislikedTag")]
        public Dictionary<string, object> DeleteUserDislikedTag([FromBody] DeleteUserDislikedTagRequest request)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var user = UserHelper.GetUserFromContext(HttpContext);
            UserBiz userBiz = new UserBiz();
            userBiz.DeleteUserDislikedTag(user.Code, request.data.TagCode);
            dic.Add("status", 200);
            dic.Add("message", "成功");
            return dic;
        }

        /// <summary>
        /// 添加喜欢的作品。
        /// </summary>
        /// <param name="request">作品参数。</param>
        /// <returns>操作结果。</returns>
        [Authorize(false), HttpPost("AddUserLikeWork")]
        public Dictionary<string, object> AddUserLikeWork([FromBody] AddUserLikeWorkRequest request)
        {
            var dic = new Dictionary<string, object>();
            var user = UserHelper.GetUserFromContext(HttpContext);
            var WorkBiz = new WorkBiz();
            var work = WorkBiz.GetWorkByGetWorkCode(request.data.WorkCode) ?? throw new CustomException("作品不存在！");

            var userBiz = new UserBiz();
            userBiz.GetUserLikeWork(user.Code, work.Code);

            userBiz.AddUserLikeWork(new UserLikeWork
            {
                WorkCode = request.data.WorkCode,
                UserCode = user.Code,
                UserName = user.Username,
                WorkTitle = work.Title
            });

            dic.Add("status", 200);
            dic.Add("message", "成功");
            return dic;
        }

        /// <summary>
        /// 获取喜欢的作品列表。
        /// </summary>
        /// <returns>喜欢作品集合。</returns>
        [Authorize(false), HttpPost("GetUserLikeWork")]
        public Dictionary<string, object> GetUserLikeWork()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var user = UserHelper.GetUserFromContext(HttpContext);
            UserBiz userBiz = new UserBiz();
            var list = userBiz.GetUserLikeWorkByUserId(user.Code);
            dic.Add("status", 200);
            dic.Add("message", "成功");
            dic.Add("data", list.Select(i => new { i.WorkCode, i.UserCode, i.WorkTitle }));
            return dic;
        }

        /// <summary>
        /// 获取用户信息（可通过 ucode 查看指定用户）。
        /// </summary>
        /// <param name="ucode">用户编码；0 表示当前用户。</param>
        /// <returns>用户信息。</returns>
        [Authorize(false), HttpGet("GetUserInfo")]
        public Dictionary<string, object> GetUserInfo(long ucode = 0)
        {
            var dic = new Dictionary<string, object>();
            var user = UserHelper.GetUserFromContext(HttpContext);
            if (ucode > 0)
            {
                user = new UserBiz().GetUserByCode(ucode);
            }
            else
            {
                if (user == null) throw new CustomException("用户未登录！");
            }

            dic.Add("status", 200);
            dic.Add("message", "成功");
            dic.Add("data", new
            {
                user.Code,
                user.Username,
                user.Email,
                user.Role,
                user.ExamineCount,
                user.Ao3Url,
                user.XUrl,
                user.WeiboUrl,
                user.LofterUrl,
                user.Gender
            });
            return dic;
        }

        /// <summary>
        /// 上传并更新用户头像（覆盖保存 PNG）。
        /// </summary>
        /// <param name="avatar">头像文件。</param>
        /// <returns>头像地址。</returns>
        [Authorize(false), HttpPost("UpdateAvatarByFile")]
        public async Task<Dictionary<string, object>> UpdateAvatarByFile(IFormFile avatar)
        {
            var dic = new Dictionary<string, object>();
            var user = UserHelper.GetUserFromContext(HttpContext);
            if (avatar == null || avatar.Length == 0) throw new CustomException("请上传头像文件！");

            var userDir = Path.Combine("wwwroot", "avatars", user.Code.ToString());
            Directory.CreateDirectory(userDir);
            var fileName = "avatar.png";
            var savePath = Path.Combine(userDir, fileName);

            using (var stream = avatar.OpenReadStream())
            using (var image = await Image.LoadAsync(stream))
            {
                await image.SaveAsPngAsync(savePath, new PngEncoder());
            }

            var avatarUrl = $"/avatars/{user.Code}/{fileName}";
            dic.Add("status", 200);
            dic.Add("message", "头像更新成功");
            dic.Add("avatarUrl", avatarUrl);
            return dic;
        }

        /// <summary>
        /// 更新用户资料请求。
        /// </summary>
        public class UpdateUserInfoRequest
        {
            /// <summary>
            /// 请求数据。
            /// </summary>
            public UpdateUserInfoData data { get; set; }

            /// <summary>
            /// 用户资料字段。
            /// </summary>
            public class UpdateUserInfoData
            {
                /// <summary>AO3 链接。</summary>
                public string Ao3Url { get; set; }

                /// <summary>X(Twitter) 链接。</summary>
                public string XUrl { get; set; }

                /// <summary>微博链接。</summary>
                public string WeiboUrl { get; set; }

                /// <summary>Lofter 链接。</summary>
                public string LofterUrl { get; set; }

                /// <summary>性别/性向。</summary>
                public string Gender { get; set; }

                /// <summary>个人简介。</summary>
                public string Profile { get; set; }
            }
        }

        /// <summary>
        /// 更新当前登录用户资料。
        /// </summary>
        /// <param name="request">资料内容。</param>
        /// <returns>操作结果。</returns>
        [Authorize(false), HttpPost("UpdateUserInfo")]
        public Dictionary<string, object> UpdateUserInfo([FromBody] UpdateUserInfoRequest request)
        {
            var dic = new Dictionary<string, object>();
            var user = UserHelper.GetUserFromContext(HttpContext) ?? throw new CustomException("用户未登录！");

            var userBiz = new UserBiz();
            user.Ao3Url = request.data.Ao3Url;
            user.XUrl = request.data.XUrl;
            user.WeiboUrl = request.data.WeiboUrl;
            user.LofterUrl = request.data.LofterUrl;
            user.Gender = request.data.Gender;
            user.Profile = request.data.Profile;
            userBiz.UpdateUser(user);

            dic.Add("status", 200);
            dic.Add("message", "用户信息更新成功");
            return dic;
        }

        /// <summary>
        /// 分页获取指定用户作品列表。
        /// </summary>
        /// <param name="uCode">用户编码。</param>
        /// <param name="pageIndex">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页数量。</param>
        /// <returns>分页作品数据。</returns>
        [HttpGet("GetUserWorks")]
        public Dictionary<string, object> GetUserWorks(long uCode = 0, int pageIndex = 0, int pageSize = 0)
        {
            var dic = new Dictionary<string, object>();
            if (uCode == 0 || pageIndex == 0 || pageSize == 0)
                throw new CustomException("用户编号、页码和每页数量不能为空或为0！");

            var userBiz = new UserBiz();
            var user = userBiz.GetUserByCode(uCode) ?? throw new CustomException("没有该用户！");

            var workBiz = new WorkBiz();
            var works = workBiz.GetWorksByUserCode(uCode, pageIndex, pageSize);

            dic.Add("status", 200);
            dic.Add("message", "成功");
            dic.Add("data", new
            {
                total = works.Total,
                pageIndex,
                pageSize,
                works = works.Data
            });
            return dic;
        }
    }
}