namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// 更新头像请求（预留结构）。
    /// </summary>
    public class UpdateAvatarByFileRequest
    {
        /// <summary>
        /// 请求数据。
        /// </summary>
        public RetrievePasswordRequestData data { get; set; }

        /// <summary>
        /// 预留字段（当前未使用）。
        /// </summary>
        public class RetrievePasswordRequestData
        {
            /// <summary>作品编码（未使用）。</summary>
            public long WorkCode { get; set; }
        }
    }
}
