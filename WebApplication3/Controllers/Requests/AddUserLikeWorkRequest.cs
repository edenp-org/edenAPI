namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// ���ϲ������Ʒ����
    /// </summary>
    public class AddUserLikeWorkRequest
    {
        /// <summary>
        /// �������ݡ�
        /// </summary>
        public AddUserLikeWorkRequestData data { get; set; }

        /// <summary>
        /// ���ϲ����Ʒ������
        /// </summary>
        public class AddUserLikeWorkRequestData
        {
            /// <summary>��Ʒ���롣</summary>
            public long WorkCode { get; set; }
        }
    }
}
