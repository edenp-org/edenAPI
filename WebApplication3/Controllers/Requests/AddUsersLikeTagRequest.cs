namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// ���ϲ����ǩ����
    /// </summary>
    public class AddUsersLikeTagRequest
    {
        /// <summary>
        /// �������ݡ�
        /// </summary>
        public AddUsersLikeTagRequestData data { get; set; }

        /// <summary>
        /// ���ϲ����ǩ������
        /// </summary>
        public class AddUsersLikeTagRequestData
        {
            /// <summary>��ǩΨһ���롣</summary>
            public long TagCode { get; set; }
        }
    }
}
