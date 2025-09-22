namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// 添加喜欢标签请求。
    /// </summary>
    public class AddUsersLikeTagRequest
    {
        /// <summary>
        /// 请求数据。
        /// </summary>
        public AddUsersLikeTagRequestData data { get; set; }

        /// <summary>
        /// 添加喜欢标签参数。
        /// </summary>
        public class AddUsersLikeTagRequestData
        {
            /// <summary>标签唯一编码。</summary>
            public long TagCode { get; set; }
        }
    }
}
