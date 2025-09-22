namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// ����������֤������
    /// </summary>
    public class SendEmailVerificationCodeRequest
    {
        /// <summary>
        /// �������ݡ�
        /// </summary>
        public SendEmailVerificationCodeRequestData data { get; set; }

        /// <summary>
        /// ����������֤�������
        /// </summary>
        public class SendEmailVerificationCodeRequestData
        {
            /// <summary>Ŀ�����䡣</summary>
            public string Email { get; set; }

            /// <summary>ͼ����֤�� ID��</summary>
            public string CaptchaId { get; set; }

            /// <summary>ͼ����֤�����롣</summary>
            public string CaptchaInput { get; set; }
        }
    }
}
