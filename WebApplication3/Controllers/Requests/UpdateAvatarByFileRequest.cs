namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// ����ͷ������Ԥ���ṹ����
    /// </summary>
    public class UpdateAvatarByFileRequest
    {
        /// <summary>
        /// �������ݡ�
        /// </summary>
        public RetrievePasswordRequestData data { get; set; }

        /// <summary>
        /// Ԥ���ֶΣ���ǰδʹ�ã���
        /// </summary>
        public class RetrievePasswordRequestData
        {
            /// <summary>��Ʒ���루δʹ�ã���</summary>
            public long WorkCode { get; set; }
        }
    }
}
