namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// 添加不喜欢标签请求。
    /// </summary>
    public class AddUserDislikedTagRequest
    {
        /// <summary>
        /// 请求数据。
        /// </summary>
        public AddUserDislikedTagRequestData data { get; set; }

        /// <summary>
        /// 添加不喜欢标签参数。
        /// </summary>
        public class AddUserDislikedTagRequestData
        {
            /// <summary>标签唯一编码。</summary>
            public long TagCode { get; set; }
        }
    }
}
