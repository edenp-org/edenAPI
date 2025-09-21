namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// �һػ��޸���������
    /// </summary>
    public class RetrievePasswordRequest
    {
        /// <summary>
        /// �������ݡ�
        /// </summary>
        public RetrievePasswordRequestData data { get; set; }

        /// <summary>
        /// �һػ��޸����������
        /// </summary>
        public class RetrievePasswordRequestData
        {
            /// <summary>���䡣</summary>
            public string Email { get; set; }

            /// <summary>�����루�ѵ�¼�޸�ʱ���룩��</summary>
            public string OldPassword { get; set; }

            /// <summary>�����롣</summary>
            public string Password { get; set; }

            /// <summary>ȷ�������롣</summary>
            public string PasswordSecond { get; set; }

            /// <summary>������֤�롣</summary>
            public string Captcha { get; set; }
        }
    }
}
