using FreeSql.DataAnnotations;

namespace WebApplication3.Models.DB
{
    /// <summary>
    /// 用户类，表示数据库中的用户表
    /// </summary>
    [Index("uk_code", "Code", true)]
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

        /// <summary>
        /// 密码的混淆码（加盐值）
        /// </summary>
        public string Confuse { get; set; }
        /// <summary>
        /// 用户角色
        /// </summary>
        public int Role { get; set; } 
    }
}
