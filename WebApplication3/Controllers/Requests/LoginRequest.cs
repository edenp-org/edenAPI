namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// ��¼����������װ��
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// ʵ���������ݡ�
        /// </summary>
        public LoginRequestData data { get; set; }

        /// <summary>
        /// ��¼����������û����������ѡһ����
        /// </summary>
        public class LoginRequestData
        {
            /// <summary>���루ǰ�����ģ����ƴ���κ��ϣ����</summary>
            public string Password { get; set; }

            /// <summary>ͼ����֤�� ID��</summary>
            public string CaptchaId { get; set; }

            /// <summary>ͼ����֤���������ݡ�</summary>
            public string CaptchaInput { get; set; }

            /// <summary>�û������û�����¼ʱ��д����</summary>
            public string Uname { get; set; }

            /// <summary>���䣨�����¼ʱ��д����</summary>
            public string Email { get; set; }
        }
    }
}
