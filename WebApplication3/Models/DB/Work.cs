using FreeSql.DataAnnotations;

namespace WebApplication3.Models.DB
{
    [Index("uk_code", "Code", true)]
    public class Work
    {
        /// <summary>
        /// 作品ID，主键，自增
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }

        /// <summary>
        /// 作品标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 作品描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 作品内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 作者ID，外键，关联用户表
        /// </summary>
        public long AuthorCode { get; set; }

        public string AuthorName { get; set; }

        public string Tags { get; set; }
        public long Code { get; set; }
    }
}
