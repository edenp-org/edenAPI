using FreeSql.DataAnnotations;

namespace WebApplication3.Models.DB
{
    /// <summary>
    /// 表示用户Token的实体类
    /// </summary>
    [Table(Name = "UserTokens")]
    public class UserToken
    {
        /// <summary>
        /// 主键，自增列
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }

        /// <summary>
        /// 用户名，最大长度为256，不能为空
        /// </summary>
        [Column(StringLength = 256, IsNullable = false)]
        public string Username { get; set; }

        /// <summary>
        /// Token，不能为空
        /// </summary>
        [Column(IsNullable = false)]
        public string Token { get; set; }

        /// <summary>
        /// Token的作用，最大长度为256，不能为空
        /// </summary>
        [Column(StringLength = 256, IsNullable = false)]
        public string Purpose { get; set; }

        /// <summary>
        /// Token的过期时间，不能为空
        /// </summary>
        [Column(IsNullable = false)]
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Token的创建时间
        /// </summary>
        [Column(IsNullable = false)]
        public DateTime CreatedAt { get; set; }
     }
}
