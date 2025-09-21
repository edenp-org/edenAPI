namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// 删除喜欢标签请求。
    /// </summary>
    public class DeleteUsersLikeTagRequest
    {
        /// <summary>
        /// 请求数据。
        /// </summary>
        public DeleteUsersLikeTagRequestRequestData data { get; set; }

        /// <summary>
        /// 删除喜欢标签参数。
        /// </summary>
        public class DeleteUsersLikeTagRequestRequestData
        {
            /// <summary>标签唯一编码。</summary>
            public long TagCode { get; set; }
        }
    }
}
