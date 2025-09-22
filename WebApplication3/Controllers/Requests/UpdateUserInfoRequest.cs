namespace WebApplication3.Controllers.Requests
{
    /// <summary>
    /// 更新用户资料请求。
    /// </summary>
    public class UpdateUserInfoRequest
    {
        /// <summary>
        /// 请求数据。
        /// </summary>
        public UpdateUserInfoData data { get; set; }

        /// <summary>
        /// 用户资料字段。
        /// </summary>
        public class UpdateUserInfoData
        {
            /// <summary>AO3 链接。</summary>
            public string Ao3Url { get; set; }

            /// <summary>X(Twitter) 链接。</summary>
            public string XUrl { get; set; }

            /// <summary>微博链接。</summary>
            public string WeiboUrl { get; set; }

            /// <summary>Lofter 链接。</summary>
            public string LofterUrl { get; set; }

            /// <summary>性别/性向。</summary>
            public string Gender { get; set; }

            /// <summary>个人简介。</summary>
            public string Profile { get; set; }
        }
    }
}
