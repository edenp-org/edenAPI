namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// ע������
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// �������ݡ�
        /// </summary>
        public RegisterRequestData data { get; set; }

        /// <summary>
        /// ע�������
        /// </summary>
        public class RegisterRequestData
        {
            /// <summary>���䡣</summary>
            public string Email { get; set; }

            /// <summary>�û�����</summary>
            public string Uname { get; set; }

            /// <summary>���롣</summary>
            public string Password { get; set; }

            /// <summary>ȷ�����롣</summary>
            public string PasswordSecond { get; set; }

            /// <summary>������֤�롣</summary>
            public string Captcha { get; set; }
        }
    }
}
