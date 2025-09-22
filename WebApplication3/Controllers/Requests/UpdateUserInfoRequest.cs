namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// �����û���������
    /// </summary>
    public class UpdateUserInfoRequest
    {
        /// <summary>
        /// �������ݡ�
        /// </summary>
        public UpdateUserInfoData data { get; set; }

        /// <summary>
        /// �û������ֶΡ�
        /// </summary>
        public class UpdateUserInfoData
        {
            /// <summary>AO3 ���ӡ�</summary>
            public string Ao3Url { get; set; }

            /// <summary>X(Twitter) ���ӡ�</summary>
            public string XUrl { get; set; }

            /// <summary>΢�����ӡ�</summary>
            public string WeiboUrl { get; set; }

            /// <summary>Lofter ���ӡ�</summary>
            public string LofterUrl { get; set; }

            /// <summary>�Ա�/����</summary>
            public string Gender { get; set; }

            /// <summary>���˼�顣</summary>
            public string Profile { get; set; }
        }
    }
}
