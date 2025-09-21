namespace WebApplication3.Controllers.Requests
{
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
}
