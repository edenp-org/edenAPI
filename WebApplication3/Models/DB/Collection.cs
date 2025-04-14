using FreeSql.DataAnnotations;

namespace WebApplication3.Models.DB
{
    /// <summary>
    /// 合集类，表示数据库中的合集表
    /// </summary>
    public class Collection
    {
        /// <summary>
        /// 合集ID，主键，自增
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }

        /// <summary>
        /// 合集名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 合集描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        public long Code { get; set; }

        /// <summary>
        /// 创建者Code
        /// </summary>
        public long UCode { get; set; }

        /// <summary>
        /// 创建者名称
        /// </summary>
        public string UName { get; set; }
    }
}