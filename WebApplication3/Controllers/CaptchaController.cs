using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaptchaController : Controller
    {
        private readonly ICaptcha _captcha;

        public CaptchaController(ICaptcha captcha)
        {
            _captcha = captcha;
        }

        [HttpGet]
        public Dictionary<string, object> Captcha()
        {
            try
            {
                string id = Guid.NewGuid().ToString();
                var info = _captcha.Generate(id);
                // 有多处验证码且过期时间不一样，可传第二个参数覆盖默认配置。
                //var info = _captcha.Generate(id,120);
                return new Dictionary<string, object>()
                {
                    { "status", 200},
                    { "CaptchaID",id },
                    { "Base64", info.Base64 },
                    { "message", "成功" }
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>()
                {
                    { "status", 400},
                    { "message", ex.Message }
                };

            }
        }

        /// <summary>
        /// 演示时使用HttpGet传参方便，这里仅做返回处理
        /// </summary>
        [HttpGet("validate")]
        public bool Validate(string id, string code)
        {
            return _captcha.Validate(id, code);
        }

        /// <summary>
        /// 多次校验（https://gitee.com/pojianbing/lazy-captcha/issues/I4XHGM）
        /// 演示时使用HttpGet传参方便，这里仅做返回处理
        /// </summary>
        [HttpGet("validate2")]
        public bool Validate2(string id, string code)
        {
            return _captcha.Validate(id, code, false);
        }
    }
}
