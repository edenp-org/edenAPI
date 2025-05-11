using FreeSql.DataAnnotations;

namespace WebApplication3.Models.DB;

public class ExamineRecord
{

    [Column(IsIdentity = true, IsPrimary = true)]
    public int Id { get; set; }

    public long WorkCode { get; set; }

    [Column(StringLength = -1)]
    public string Result { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}