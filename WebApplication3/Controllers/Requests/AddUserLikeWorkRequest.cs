namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// 添加喜欢的作品请求。
    /// </summary>
    public class AddUserLikeWorkRequest
    {
        /// <summary>
        /// 请求数据。
        /// </summary>
        public AddUserLikeWorkRequestData data { get; set; }

        /// <summary>
        /// 添加喜欢作品参数。
        /// </summary>
        public class AddUserLikeWorkRequestData
        {
            /// <summary>作品编码。</summary>
            public long WorkCode { get; set; }
        }
    }
}
