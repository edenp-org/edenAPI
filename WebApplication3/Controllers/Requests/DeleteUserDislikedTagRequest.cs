namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// ɾ����ϲ����ǩ����
    /// </summary>
    public class DeleteUserDislikedTagRequest
    {
        /// <summary>
        /// �������ݡ�
        /// </summary>
        public DeleteUserDislikedTagRequestData data { get; set; }

        /// <summary>
        /// ɾ����ϲ����ǩ������
        /// </summary>
        public class DeleteUserDislikedTagRequestData
        {
            /// <summary>��ǩΨһ���롣</summary>
            public long TagCode { get; set; }
        }
    }
}
