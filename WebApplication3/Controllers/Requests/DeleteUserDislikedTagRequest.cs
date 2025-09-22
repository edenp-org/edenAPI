namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// 删除不喜欢标签请求。
    /// </summary>
    public class DeleteUserDislikedTagRequest
    {
        /// <summary>
        /// 请求数据。
        /// </summary>
        public DeleteUserDislikedTagRequestData data { get; set; }

        /// <summary>
        /// 删除不喜欢标签参数。
        /// </summary>
        public class DeleteUserDislikedTagRequestData
        {
            /// <summary>标签唯一编码。</summary>
            public long TagCode { get; set; }
        }
    }
}
