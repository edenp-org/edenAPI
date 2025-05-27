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

        /// <summary>
        /// 作者名称
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime ExamineDate { get; set; }

        /// <summary>
        /// 作品编码
        /// </summary>
        public long Code { get; set; }

        /// <summary>
        /// 合集Code，为0表示不属于任何合集
        /// </summary>
        public long CollectionCode { get; set; }

        /// <summary>
        /// 合集排序
        /// </summary>
        public long CollectionOrder { get; set; }

        /// <summary>
        /// 是否审核
        /// </summary>

        public int IsExamine { get; set; } = 0;

        /// <summary>
        /// 是否定时发布
        /// </summary>
        public bool IsScheduledRelease { get; set; } = false;

        /// <summary>
        /// 定时发布时间
        /// </summary>
        public DateTime ScheduledReleaseTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 是否已发布
        /// </summary>
        public bool IsPublished { get; set; } = false;
    }
}