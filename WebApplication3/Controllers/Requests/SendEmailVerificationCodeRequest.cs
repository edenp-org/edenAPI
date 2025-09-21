namespace WebApplication3.Controllers.Requests
{
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
}
