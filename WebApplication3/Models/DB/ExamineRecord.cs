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

    public string DataSHA265 { get; set; }

    public string UserName { get; set; }
    
    public long UserCode { get; set; }

    public bool IsOk { get; set; } = false;

    public bool IsAi { get; set; } = true;
}