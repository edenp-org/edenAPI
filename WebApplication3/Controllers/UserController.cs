using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Biz;
using WebApplication3.Foundation;
using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;
using WebApplication3.Sundry;

namespace WebApplication3.Controllers
{
    [Route("User")]
    public class UserController : Controller
    {
        public class LoginRequest
        {
            public string Password { get; set; }
            public string CaptchaId { get; set; }
            public string CaptchaInput { get; set; }
            public string Uname { get; set; }
            public string Email { get; set; }
        }

        public class SendEmailVerificationCodeRequest
        {
            public string Email { get; set; }
            public string CaptchaId { get; set; }
            public string CaptchaInput { get; set; }
        }

        public class RegisterRequest
        {
            public string Email { get; set; }
            public string Uname { get; set; }
            public string Password { get; set; }
            public string PasswordSecond { get; set; }
            public string Captcha { get; set; }
        }

        public class AddUsersLikeTagRequest
        {
            public long TagCode { get; set; }
        }

        public class DeleteUsersLikeTagRequest
        {
            public long TagCode { get; set; }
        }

        public class AddUserDislikedTagRequest
        {
            public long TagCode { get; set; }
        }

        public class AddUserLikeWorkRequest
        {
            public long WorkCode { get; set; }
        }

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
            try
            {
                // 验证输入参数
                if (string.IsNullOrEmpty(request.Password)) throw new Exception("请输入密码！");
                if (string.IsNullOrEmpty(request.CaptchaId)) throw new Exception("未获取到验证码ID！");
                if (string.IsNullOrEmpty(request.CaptchaInput)) throw new Exception("请输入验证码！");
                if (string.IsNullOrEmpty(request.Uname) && string.IsNullOrEmpty(request.Email)) throw new Exception("请选择登录方式");
                if (!string.IsNullOrEmpty(request.Uname) && !string.IsNullOrEmpty(request.Email)) throw new Exception("请选择登录方式");

                // 验证验证码
                if (!_captcha.Validate(request.CaptchaId, request.CaptchaInput)) throw new Exception("验证码错误！");

                var userBiz = new UserBiz();
                var userTokenBiz = new UserTokenBiz();

                // 根据用户名或邮箱获取用户
                var user = !string.IsNullOrEmpty(request.Email) ? userBiz.GetUserByEmail(request.Email) : userBiz.GetUserByUname(request.Uname);
                if (user == null) throw new Exception("用户名或邮箱或密码不存在！");
                if (!user.Password.Equals(
                        EncryptionHelper.ComputeSHA256(EncryptionHelper.ComputeSHA256(request.Password) + user.Confuse)))
                    throw new Exception("用户名或邮箱或密码不存在！");

                // 生成Token
                var token = TokenService.GenerateToken(user.Username, user.Role.ToString(), "登录");

                // 保存Token
                userTokenBiz.Add(new UserToken
                {
                    Expiration = DateTime.Now.AddHours(ConfigHelper.GetInt("TokenExpirationHours")),
                    CreatedAt = DateTime.Now,
                    Purpose = "登录",
                    Token = token,
                    Username = user.Username
                });

                dic.Add("status", 200);
                dic.Add("message", "登录成功！");
                dic.Add("data", token);
            }
            catch (Exception ex)
            {
                dic.Add("status", 400);
                dic.Add("message", ex.Message);
            }

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
            try
            {
                // 验证输入参数
                if (string.IsNullOrEmpty(request.Email)) throw new Exception("请输入邮箱！");
                if (string.IsNullOrEmpty(request.CaptchaId)) throw new Exception("请输入验证码ID！");
                if (string.IsNullOrEmpty(request.CaptchaInput)) throw new Exception("请输入验证码！");
                if (!_captcha.Validate(request.CaptchaId, request.CaptchaInput)) throw new Exception("错误的验证码");

                // 生成验证码
                var token = new Random().Next(100000, 999999).ToString();

                // 保存验证码
                var userTokenBiz = new UserTokenBiz();
                userTokenBiz.Add(new UserToken
                {
                    Expiration = DateTime.Now.AddMinutes(5),
                    CreatedAt = DateTime.Now,
                    Purpose = "注册",
                    Token = token,
                    Username = request.Email
                });

                // 发送邮件
                EmailHelper.SendEmail(request.Email, "邮箱验证", $"您的验证码是：{token} \r\n 该验证码5分钟后过期!");

                dic.Add("status", 200);
                dic.Add("message", "验证码已发送至邮箱，请查收！");
            }
            catch (Exception ex)
            {
                dic.Add("status", 400);
                dic.Add("message", ex.Message);
            }

            return dic;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="request">注册请求参数</param>
        /// <returns>注册结果</returns>
        public Dictionary<string, object> Register([FromBody] RegisterRequest request)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                // 验证输入参数
                if (string.IsNullOrEmpty(request.Email)) throw new Exception("请输入邮箱！");
                if (string.IsNullOrEmpty(request.Uname)) throw new Exception("请输入用户名！");
                if (string.IsNullOrEmpty(request.Password)) throw new Exception("请输入密码！");
                if (string.IsNullOrEmpty(request.PasswordSecond)) throw new Exception("请输入确认密码！");
                if (string.IsNullOrEmpty(request.Captcha)) throw new Exception("请输入验证码！");
                if (request.Password != request.PasswordSecond) throw new Exception("两次密码不一致！");
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Uname) || string.IsNullOrEmpty(request.Password))
                    throw new Exception("请填写完整信息！");

                var userBiz = new UserBiz();
                var userTokenBiz = new UserTokenBiz();

                // 检查邮箱和用户名是否已被注册
                if (userBiz.GetUserByEmail(request.Email) != null) throw new Exception("邮箱已被注册！");
                if (userBiz.GetUserByUname(request.Uname) != null) throw new Exception("用户名已被注册！");

                // 验证验证码
                var userToken = userTokenBiz.GetTokenByUserAndPurpose(request.Email, "注册")
                    .FirstOrDefault(u => u.Token.Equals(request.Captcha));
                if (userToken == null) throw new Exception("验证码错误或已过期！");

                // 注册新用户
                lock (_codeLock)
                {
                    var newCodeUser = userBiz.GetNewCode();
                    var confuse = new Random().Next(100000, 999999).ToString("X");
                    userBiz.Add(new User
                    {
                        Email = request.Email,
                        Username = request.Uname,
                        Password = EncryptionHelper.ComputeSHA256(EncryptionHelper.ComputeSHA256(request.Password) + confuse),
                        Code = newCodeUser,
                        Confuse = confuse,
                        Role = 2
                    });
                }

                dic.Add("status", 200);
                dic.Add("message", "注册成功！");
            }
            catch (Exception ex)
            {
                dic.Add("status", 400);
                dic.Add("message", ex.Message);
            }

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
            try
            {
                // 获取用户信息
                var userId = HttpContext.Items["UserId"]?.ToString();
                var Uname = HttpContext.Items["Uname"]?.ToString();
                var UCode = HttpContext.Items["Code"]?.ToString();
                if (string.IsNullOrEmpty(userId)) throw new Exception("用户未授权！");
                if (!long.TryParse(UCode, out long UCodeLong)) throw new Exception("用户未授权！");

                if (new TagBiz().GetTagByCode(request.TagCode) == null) throw new Exception("标签不存在！");
                if (new UserBiz().GetUserByCode(UCodeLong) == null) throw new Exception("用户不存在！");

                UserBiz userBiz = new UserBiz();
                userBiz.AddUserFavoriteTag(new UserFavoriteTag
                {
                    TagCode = request.TagCode,
                    UserCode = UCodeLong
                });
                dic.Add("status", 200);
                dic.Add("message", "成功");
            }
            catch (Exception ex)
            {
                dic.Add("status", 400);
                dic.Add("message", ex.Message);
            }

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
            try
            {
                // 获取用户信息
                var userId = HttpContext.Items["UserId"]?.ToString();
                var Uname = HttpContext.Items["Uname"]?.ToString();
                var UCode = HttpContext.Items["Code"]?.ToString();

                if (!long.TryParse(UCode, out long UCodeLong)) throw new Exception("用户未授权！");
                UserBiz userBiz = new UserBiz();
                userBiz.DeleteUsersLikeTag(UCodeLong, request.TagCode);

                dic.Add("status", "200");
                dic.Add("message", "成功");
            }
            catch (Exception ex)
            {
                dic.Add("status", "400");
                dic.Add("message", ex.Message);
            }

            return dic;
        }

        /// <summary>
        /// 获取用户喜欢的标签
        /// </summary>
        /// <returns>标签内容</returns>
        /// <exception cref="Exception"></exception>
        [Authorize(false), HttpGet("GetUserUsersLikeTag")]
        public Dictionary<string,object> GetUserUsersLikeTag()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                // 获取用户信息
                var userId = HttpContext.Items["UserId"]?.ToString();
                var Uname = HttpContext.Items["Uname"]?.ToString();
                var UCode = HttpContext.Items["Code"]?.ToString();

                if (!long.TryParse(UCode, out long UCodeLong)) throw new Exception("用户未授权！");
                UserBiz userBiz = new UserBiz();
                var list = userBiz.GetUserFavoriteTagByUserId(UCodeLong);
                dic.Add("status", "200");
                dic.Add("message", "成功");
                dic.Add("data", list.Select(i=> new { i.UserCode,i.TagCode}).ToList());
            }
            catch (Exception ex)
            {
                dic.Add("status", "400");
                dic.Add("message", ex.Message);
            }

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
            try
            {
                // 获取用户信息
                var userId = HttpContext.Items["UserId"]?.ToString();
                var Uname = HttpContext.Items["Uname"]?.ToString();
                var UCode = HttpContext.Items["Code"]?.ToString();
                if (string.IsNullOrEmpty(userId)) throw new Exception("用户未授权！");
                if (!long.TryParse(UCode, out long UCodeLong)) throw new Exception("用户未授权！");
                if (new TagBiz().GetTagByCode(request.TagCode) == null) throw new Exception("标签不存在！");
                if (new UserBiz().GetUserByCode(UCodeLong) == null) throw new Exception("用户不存在！");

                UserBiz userBiz = new UserBiz();
                userBiz.AddUserDislikedTag(new UserDislikedTag
                {
                    TagCode = request.TagCode,
                    UserCode = UCodeLong
                });
                dic.Add("status", 200);
                dic.Add("message", "成功");
            }
            catch (Exception ex)
            {
                dic.Add("status", 400);
                dic.Add("message", ex.Message);
            }

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
            try
            {
                // 获取用户信息
                var userId = HttpContext.Items["UserId"]?.ToString();
                var Uname = HttpContext.Items["Uname"]?.ToString();
                var UCode = HttpContext.Items["Code"]?.ToString();
                if (string.IsNullOrEmpty(userId)) throw new Exception("用户未授权！");
                if (!long.TryParse(UCode, out long UCodeLong)) throw new Exception("用户未授权！");

                // 检查作品是否存在
                var WorkBiz = new WorkBiz();
                if (WorkBiz.GetWorkByGetWorkCode(request.WorkCode) == null) throw new Exception("作品不存在！");

                // 检查用户是否存在
                var userBiz = new UserBiz();
                if (userBiz.GetUserByCode(UCodeLong) == null) throw new Exception("用户不存在！");

                // 添加用户喜欢的作品
                userBiz.AddUserLikeWork(new UserLikeWork
                {
                    WorkCode = request.WorkCode,
                    UserCode = UCodeLong
                });

                dic.Add("status", 200);
                dic.Add("message", "成功");
            }
            catch (Exception ex)
            {
                dic.Add("status", 400);
                dic.Add("message", ex.Message);
            }

            return dic;
        }
    }
}