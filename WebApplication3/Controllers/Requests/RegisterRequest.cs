namespace WebApplication3.Controllers.Requests
{
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
}
