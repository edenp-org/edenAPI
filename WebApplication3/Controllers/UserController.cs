using System.Security.Claims;
using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Biz;
using WebApplication3.Foundation;
using WebApplication3.Foundation.Exceptions;
using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;
using WebApplication3.Sundry;

namespace WebApplication3.Controllers
{
    [Route("User")]
    public class UserController : Controller
    {
        #region 入参类型

        public class LoginRequest
        {
            public LoginRequestData data { get; set; }

            public class LoginRequestData
            {
                public string Password { get; set; }
                public string CaptchaId { get; set; }
                public string CaptchaInput { get; set; }
                public string Uname { get; set; }
                public string Email { get; set; }
            }
        }

        public class SendEmailVerificationCodeRequest
        {
            public SendEmailVerificationCodeRequestData data { get; set; }

            public class SendEmailVerificationCodeRequestData
            {
                public string Email { get; set; }
                public string CaptchaId { get; set; }
                public string CaptchaInput { get; set; }
            }
        }

        public class RegisterRequest
        {
            public RegisterRequestData data { get; set; }

            public class RegisterRequestData
            {
                public string Email { get; set; }
                public string Uname { get; set; }
                public string Password { get; set; }
                public string PasswordSecond { get; set; }
                public string Captcha { get; set; }
            }
        }

        public class AddUsersLikeTagRequest
        {
            public AddUsersLikeTagRequestData data { get; set; }

            public class AddUsersLikeTagRequestData
            {
                public long TagCode { get; set; }
            }
        }

        public class DeleteUsersLikeTagRequest
        {
            public DeleteUsersLikeTagRequestRequestData data { get; set; }

            public class DeleteUsersLikeTagRequestRequestData
            {
                public long TagCode { get; set; }
            }
        }

        public class AddUserDislikedTagRequest
        {
            public AddUserDislikedTagRequestData data { get; set; }

            public class AddUserDislikedTagRequestData
            {
                public long TagCode { get; set; }
            }
        }

        public class DeleteUserDislikedTagRequest
        {
            public DeleteUserDislikedTagRequestData data { get; set; }

            public class DeleteUserDislikedTagRequestData
            {
                public long TagCode { get; set; }
            }
        }

        public class AddUserLikeWorkRequest
        {
            public AddUserLikeWorkRequestData data { get; set; }

            public class AddUserLikeWorkRequestData
            {
                public long WorkCode { get; set; }
            }
        }

        #endregion

        private readonly ICaptcha _captcha;
        private static readonly object _codeLock = new object();

        public UserController(ICaptcha captcha)
        {
            _captcha = captcha;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="request">登录请求参数</param>
        /// <returns>登录结果</returns>
        [HttpPost("Login")]
        public Dictionary<string, object> Login([FromBody] LoginRequest request)
        {
            var dic = new Dictionary<string, object>();

            // 验证输入参数
            if (string.IsNullOrEmpty(request.data.Password)) throw new CustomException("请输入密码！");
            if (string.IsNullOrEmpty(request.data.CaptchaId)) throw new CustomException("未获取到验证码ID！");
            if (string.IsNullOrEmpty(request.data.CaptchaInput)) throw new CustomException("请输入验证码！");
            if (string.IsNullOrEmpty(request.data.Uname) && string.IsNullOrEmpty(request.data.Email)) throw new CustomException("请选择登录方式");
            if (!string.IsNullOrEmpty(request.data.Uname) && !string.IsNullOrEmpty(request.data.Email)) throw new CustomException("请选择登录方式");

            // 验证验证码
            //if (!_captcha.Validate(request.data.CaptchaId, request.data.CaptchaInput)) throw new CustomException("验证码错误！");

            var userBiz = new UserBiz();
            var userTokenBiz = new UserTokenBiz();

            // 根据用户名或邮箱获取用户
            var user = !string.IsNullOrEmpty(request.data.Email) ? userBiz.GetUserByEmail(request.data.Email) : userBiz.GetUserByUname(request.data.Uname);
            if (user == null) throw new CustomException("用户名或邮箱或密码不存在！");
            if (!user.Password.Equals(EncryptionHelper.ComputeSHA256(EncryptionHelper.ComputeSHA256(request.data.Password) + user.Confuse)))
                throw new CustomException("用户名或邮箱或密码不存在！");

            int expirationHours = ConfigHelper.GetInt("TokenExpirationHours");
            var refreshTokenExpires = DateTime.UtcNow.AddHours(expirationHours);
            var accessTokenExpires = DateTime.UtcNow.AddHours(1);

            // 生成Token
            var refreshToken = TokenService.GenerateToken(user.Username, user.Role.ToString(), "refreshToken", user.Code, refreshTokenExpires);
            var accessToken = TokenService.GenerateToken(user.Username, user.Role.ToString(), "accessToken", user.Code, accessTokenExpires);

            // 保存Token
            userTokenBiz.Add(new UserToken
            {
                Expiration = DateTime.UtcNow.AddHours(ConfigHelper.GetInt("TokenExpirationHours")),
                CreatedAt = DateTime.UtcNow,
                Purpose = "refreshToken",
                Token = refreshToken,
                Username = user.Username
            });

            var accessTokenExpiresTimeSeconds = accessTokenExpires.ToUnixTimeSeconds();

            RedisHelper.Set(user.Code + accessTokenExpiresTimeSeconds.ToString(), accessToken, (int)(accessTokenExpires - DateTime.UtcNow).TotalSeconds);

            // 设置Cookie
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true, // 仅允许通过HTTP访问，防止脚本访问
                Secure = false, // 仅在HTTPS下传输
                SameSite = SameSiteMode.Strict, // 限制跨站点请求
                Expires = DateTimeOffset.UtcNow.AddMonths(1) // 设置过期时间
            });

            dic.Add("status", 200);
            dic.Add("message", "登录成功！");
            dic.Add("data", accessToken);

            return dic;
        }


        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <returns>刷新后的访问令牌</returns>
        [HttpPost("RefreshToken")]
        public Dictionary<string, object> RefreshToken()
        {
            var dic = new Dictionary<string, object>();
            // 从请求头中获取刷新令牌
            if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                throw new CustomException("未提供刷新令牌！");
            }

            var claimsPrincipal = TokenService.ValidateToken(refreshToken);
            if (claimsPrincipal == null) throw new CustomException("鉴权失败！");

            var exp = claimsPrincipal.FindFirst(ClaimTypes.Name);
            var UCode = claimsPrincipal.FindFirst("UCode");

            if (exp == null || UCode == null || !long.TryParse(UCode.Value, out long _result)) throw new CustomException("鉴权失败！");

            var userTokenBiz = new UserTokenBiz();
            // 验证刷新令牌是否存在并有效
            var userToken = userTokenBiz.GetTokenByUserAndPurpose(exp.Value, "refreshToken");
            if (userToken == null || userToken.Where(e => e.Expiration >= DateTime.UtcNow).Count() <= 0)
            {
                throw new CustomException("刷新令牌无效或已过期！");
            }

            // 获取用户信息
            var userBiz = new UserBiz();
            var user = userBiz.GetUserByCode(_result);
            if (user == null)
            {
                throw new CustomException("用户不存在！");
            }

            // 生成新地访问令牌
            var accessTokenExpires = DateTime.UtcNow.AddHours(1);
            var accessToken = TokenService.GenerateToken(user.Username, user.Role.ToString(), "accessToken", user.Code, accessTokenExpires);

            RedisHelper.Set(user.Code + accessTokenExpires.ToUnixTimeSeconds().ToString(), accessToken, (int)(accessTokenExpires - DateTime.UtcNow).TotalSeconds);
            dic.Add("message", "令牌刷新成功！");
            dic.Add("data", accessToken);
            return dic;
        }


        /// <summary>
        /// 发送邮箱验证码
        /// </summary>
        /// <param name="request">发送验证码请求参数</param>
        /// <returns>发送结果</returns>
        [HttpPost("SendEmailVerificationCode")]
        public Dictionary<string, object> SendEmailVerificationCode([FromBody] SendEmailVerificationCodeRequest request)
        {
            var dic = new Dictionary<string, object>();
            // 验证输入参数
            if (string.IsNullOrEmpty(request.data.Email)) throw new CustomException("请输入邮箱！");
            if (string.IsNullOrEmpty(request.data.CaptchaId)) throw new CustomException("请输入验证码ID！");
            if (string.IsNullOrEmpty(request.data.CaptchaInput)) throw new CustomException("请输入验证码！");
            if (!_captcha.Validate(request.data.CaptchaId, request.data.CaptchaInput)) throw new CustomException("错误的验证码");

            // 生成验证码
            var token = new Random().Next(100000, 999999).ToString();

            // 保存验证码
            var userTokenBiz = new UserTokenBiz();
            userTokenBiz.Add(new UserToken
            {
                Expiration = DateTime.UtcNow.AddMinutes(5),
                CreatedAt = DateTime.UtcNow,
                Purpose = "注册",
                Token = token,
                Username = request.data.Email
            });

            // 发送邮件
            EmailHelper.SendEmail(request.data.Email, "邮箱验证", $"您的验证码是：{token} \r\n 该验证码5分钟后过期!");

            dic.Add("status", 200);
            dic.Add("message", "验证码已发送至邮箱，请查收！");

            return dic;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="request">注册请求参数</param>
        /// <returns>注册结果</returns>
        [HttpPost("Register")]
        public Dictionary<string, object> Register([FromBody] RegisterRequest request)
        {
            var dic = new Dictionary<string, object>();

            // 验证输入参数
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

            // 检查邮箱和用户名是否已被注册
            if (userBiz.GetUserByEmail(request.data.Email) != null) throw new CustomException("邮箱已被注册！");
            if (userBiz.GetUserByUname(request.data.Uname) != null) throw new CustomException("用户名已被注册！");

            // 验证验证码
            var userToken = userTokenBiz.GetTokenByUserAndPurpose(request.data.Email, "注册")
                .FirstOrDefault(u => u.Token.Equals(request.data.Captcha));
            if (userToken == null) throw new CustomException("验证码错误或已过期！");

            // 注册新用户
            lock (_codeLock)
            {
                var newCodeUser = userBiz.GetNewCode();
                var confuse = new Random().Next(100000, 999999).ToString("X");
                userBiz.Add(new User
                {
                    Email = request.data.Email,
                    Username = request.data.Uname,
                    Password = EncryptionHelper.ComputeSHA256(EncryptionHelper.ComputeSHA256(request.data.Password) + confuse),
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
        /// 添加用户喜欢的标签
        /// </summary>
        /// <param name="request">添加标签请求参数</param>
        /// <returns>操作结果</returns>
        [Authorize(false), HttpPost("AddUsersLikeTag")]
        public Dictionary<string, object> AddUsersLikeTag([FromBody] AddUsersLikeTagRequest request)
        {
            var dic = new Dictionary<string, object>();
            var user = UserHelper.GetUserFromContext(HttpContext);

            var tag = new TagBiz().GetTagByCode(request.data.TagCode);
            if (tag == null) throw new CustomException("标签不存在！");

            UserBiz userBiz = new UserBiz();
            var favoriteTags = userBiz.GetUserFavoriteTagByUserId(user.Code, tag.Id);
            if (favoriteTags == null) throw new CustomException("已添加该标签！");
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
        /// 删除用户喜欢的标签
        /// </summary>
        /// <param name="request">删除标签请求参数</param>
        /// <returns>操作结果</returns>
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
        /// 获取用户喜欢的标签
        /// </summary>
        /// <returns>标签内容</returns>
        /// <exception cref="Exception"></exception>
        [Authorize(false), HttpGet("GetUserUsersLikeTag")]
        public Dictionary<string, object> GetUserUsersLikeTag()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var user = UserHelper.GetUserFromContext(HttpContext);

            UserBiz userBiz = new UserBiz();
            var list = userBiz.GetUserFavoriteTagByUserId(user.Code);
            dic.Add("status", "200");
            dic.Add("message", "成功");
            dic.Add("data", list.Select(i => new
            {
                i.UserCode,
                i.TagCode,
                i.TagName
            }).ToList());
            return dic;
        }

        /// <summary>
        /// 添加用户不喜欢的标签
        /// </summary>
        /// <param name="request">添加标签请求参数</param>
        /// <returns>操作结果</returns>
        [Authorize(false), HttpPost("AddUserDislikedTag")]
        public Dictionary<string, object> AddUserDislikedTag([FromBody] AddUserDislikedTagRequest request)
        {
            var dic = new Dictionary<string, object>();
            var user = UserHelper.GetUserFromContext(HttpContext);
            var tag = new TagBiz().GetTagByCode(request.data.TagCode);
            if (tag == null) throw new CustomException("标签不存在！");

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
        /// 获取用户不喜欢的标签
        /// </summary>
        /// <returns>操作结果</returns>
        [Authorize(false), HttpGet("GetUserDislikedTag")]
        public Dictionary<string, object> GetUserDislikedTag()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var user = UserHelper.GetUserFromContext(HttpContext);
            UserBiz userBiz = new UserBiz();
            var list = userBiz.GetUserDislikedTagByUserId(user.Code);

            dic.Add("status", 200);
            dic.Add("message", "成功");
            dic.Add("data", list.Select(i => new
            {
                i.TagName,
                i.UserCode,
            }));
            return dic;
        }

        /// <summary>
        /// 删除用户不喜欢的标签
        /// </summary>
        /// <param name="request">入参</param>
        /// <returns>出参</returns>
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
        /// 添加用户喜欢的作品
        /// </summary>
        /// <param name="request">添加作品请求参数</param>
        /// <returns>操作结果</returns>
        [Authorize(false), HttpPost("AddUserLikeWork")]
        public Dictionary<string, object> AddUserLikeWork([FromBody] AddUserLikeWorkRequest request)
        {
            var dic = new Dictionary<string, object>();
            // 获取用户信息
            var user = UserHelper.GetUserFromContext(HttpContext);

            // 检查作品是否存在
            var WorkBiz = new WorkBiz();
            var work = WorkBiz.GetWorkByGetWorkCode(request.data.WorkCode);
            if (work == null) throw new CustomException("作品不存在！");

            var userBiz = new UserBiz();
            userBiz.GetUserLikeWork(user.Code, work.Code);

            // 添加用户喜欢的作品
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
    }
}