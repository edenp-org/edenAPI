namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// ɾ��ϲ����ǩ����
    /// </summary>
    public class DeleteUsersLikeTagRequest
    {
        /// <summary>
        /// �������ݡ�
        /// </summary>
        public DeleteUsersLikeTagRequestRequestData data { get; set; }

        /// <summary>
        /// ɾ��ϲ����ǩ������
        /// </summary>
        public class DeleteUsersLikeTagRequestRequestData
        {
            /// <summary>��ǩΨһ���롣</summary>
            public long TagCode { get; set; }
        }
    }
}
