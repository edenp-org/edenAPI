using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using TouchSocket.Core;
using WebApplication3.Biz;
using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;
using WebApplication3.Sundry;

namespace WebApplication3.Controllers
{
    [Route("User")]
    public class UserController : Controller
    {
        private readonly ICaptcha _captcha;
        private static readonly object _codeLock = new object();

        public UserController(ICaptcha captcha)
        {
            _captcha = captcha;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="pairs">入参</param>
        /// <returns>返回登录结果</returns>
        [HttpPost("Login")]
        public Dictionary<string, object> Login([FromBody] Dictionary<string, object> pairs)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                // 验证输入参数
                if (!pairs.TryGetValue("data", out var dataObj)) throw new Exception("没有入参！");
                var data = dataObj.ToString().FromJsonString<Dictionary<string, string>>();
                if (!data.TryGetValue("password", out var password)) throw new Exception("请输入密码！");
                if (!data.TryGetValue("captchaId", out var captchaId)) throw new Exception("未获取到验证码ID！");
                if (!data.TryGetValue("captchaInput", out var captchaCode)) throw new Exception("请输入验证码！");
                if (!data.TryGetValue("uname", out var uname)) throw new Exception("请输入用户名！");
                if (!data.TryGetValue("email", out var email)) throw new Exception("请输入邮箱！");
                if (!string.IsNullOrEmpty(uname) && !string.IsNullOrEmpty(email)) throw new Exception("请选择登录方式");
                if (string.IsNullOrEmpty(uname) && string.IsNullOrEmpty(email)) throw new Exception("请选择登录方式");

                // 验证验证码
                if (!_captcha.Validate(captchaId, captchaCode)) throw new Exception("验证码错误！");

                var userBiz = new UserBiz();
                var userTokenBiz = new UserTokenBiz();

                // 根据用户名或邮箱获取用户
                var user = !string.IsNullOrEmpty(email) ? userBiz.GetUserByEmail(email) : userBiz.GetUserByUname(uname);
                if (user == null) throw new Exception("用户名或邮箱或密码不存在！");
                if (!user.Password.Equals(EncryptionHelper.ComputeSHA256(EncryptionHelper.ComputeSHA256(password) + user.Confuse))) throw new Exception("用户名或邮箱或密码不存在！");

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
        /// 获取邮箱验证码
        /// </summary>
        /// <param name="pairs">入参</param>
        /// <returns>返回发送结果</returns>
        [HttpPost("SendEmailVerificationCode")]
        public Dictionary<string, object> SendEmailVerificationCode([FromBody] Dictionary<string, object> pairs)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                // 验证输入参数
                if (!pairs.TryGetValue("Data", out var dataObj)) throw new Exception("未获取到数据！");
                var data = dataObj.ToString().FromJsonString<Dictionary<string, string>>();
                if (!data.TryGetValue("email", out var email)) throw new Exception("请输入邮箱！");
                if (!data.TryGetValue("captchaId", out var captchaId)) throw new Exception("请输入验证码ID！");
                if (!data.TryGetValue("captchaInput", out var captchaInput)) throw new Exception("请输入验证码！");
                // if (!_captcha.Validate(captchaId, captchaInput)) throw new Exception("错误的验证码");

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
                    Username = email
                });

                // 发送邮件
                EmailHelper.SendEmail(email, "邮箱验证", $"您的验证码是：{token} \r\n 该验证码5分钟后过期!");

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
        /// 注册
        /// </summary>
        /// <param name="pairs">入参</param>
        /// <returns>返回注册结果</returns>
        [HttpPost("Register")]
        public Dictionary<string, object> Register([FromBody] Dictionary<string, object> pairs)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                // 验证输入参数
                if (!pairs.TryGetValue("Data", out var dataObj)) throw new Exception("未获取到数据！");
                var data = dataObj.ToString().FromJsonString<Dictionary<string, string>>();
                if (!data.TryGetValue("email", out var email)) throw new Exception("请输入邮箱！");
                if (!data.TryGetValue("uname", out var uname)) throw new Exception("请输入用户名！");
                if (!data.TryGetValue("password", out var password)) throw new Exception("请输入密码！");
                if (!data.TryGetValue("passwordSecond", out var passwordSecond)) throw new Exception("请输入确认密码！");
                if (!data.TryGetValue("captcha", out var captcha)) throw new Exception("请输入验证码！");
                if (password != passwordSecond) throw new Exception("两次密码不一致！");
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(uname) || string.IsNullOrEmpty(password)) throw new Exception("请填写完整信息！");

                var userBiz = new UserBiz();
                var userTokenBiz = new UserTokenBiz();

                // 检查邮箱和用户名是否已被注册
                if (userBiz.GetUserByEmail(email) != null) throw new Exception("邮箱已被注册！");
                if (userBiz.GetUserByUname(uname) != null) throw new Exception("用户名已被注册！");

                // 验证验证码
                var userToken = userTokenBiz.GetTokenByUserAndPurpose(email, "注册").FirstOrDefault(u => u.Token.Equals(captcha));
                if (userToken == null) throw new Exception("验证码错误或已过期！");

                // 注册新用户
                lock (_codeLock)
                {
                    var newCodeUser = userBiz.GetNewCode();
                    var confuse = new Random().Next(100000, 999999).ToString("X");
                    userBiz.Add(new User
                    {
                        Email = email,
                        Username = uname,
                        Password = EncryptionHelper.ComputeSHA256(EncryptionHelper.ComputeSHA256(password) + confuse),
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
    }
}
