using FreeSql.DataAnnotations;

namespace WebApplication3.Models.DB
{
    /// <summary>
    /// 标签类，表示数据库中的标签表
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// 标签ID，主键，自增
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }

        /// <summary>
        /// 标签名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public int UId { get; set; }
    }
}
