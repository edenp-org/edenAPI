namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// ��Ӳ�ϲ����ǩ����
    /// </summary>
    public class AddUserDislikedTagRequest
    {
        /// <summary>
        /// �������ݡ�
        /// </summary>
        public AddUserDislikedTagRequestData data { get; set; }

        /// <summary>
        /// ��Ӳ�ϲ����ǩ������
        /// </summary>
        public class AddUserDislikedTagRequestData
        {
            /// <summary>��ǩΨһ���롣</summary>
            public long TagCode { get; set; }
        }
    }
}
