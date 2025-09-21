namespace WebApplication3.Controllers.Requests
{
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
}
