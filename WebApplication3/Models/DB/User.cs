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
        public long Code { get; set; }

        /// <summary>
        /// 密码的混淆码（加盐值）
        /// </summary>
        public string Confuse { get; set; }

        /// <summary>
        /// 用户简介
        /// </summary>
        public string Profile { get; set; }

        /// <summary>
        /// 用户性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 老福特主页地址
        /// </summary>
        public string LofterUrl { get; set; }

        /// <summary>
        /// 微博主页地址
        /// </summary>
        public string WeiboUrl { get; set; }

        /// <summary>
        /// AO3主页地址
        /// </summary>
        public string Ao3Url { get; set; }

        /// <summary>
        /// X（推特）主页地址
        /// </summary>
        public string XUrl { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public int Role { get; set; }

        /// <summary>
        /// 审核次数，每次进行审核，都+1
        /// </summary>
        public int ExamineCount { get; set; }

        /// <summary>
        /// 上次审核时间，上次审核时间若小于当月开始时间，则重置审核次数
        /// 比如：若此值为2025/05/11 ，当前时间为 2025/05/12 则不重置
        /// 比如：若此值为2025/05/11 ，当前时间为 2025/06/01 则重置
        /// </summary>
        public DateTime LastExamineTime { get; set; } = DateTime.MinValue;

    }
}
