using FreeSql.DataAnnotations;

namespace WebApplication3.Models.DB
{
    /// <summary>
    /// 用户类，表示数据库中的用户表
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户ID，主键，自增
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 用户代码
        /// </summary>
        public string Code { get; set; }
    }
}
