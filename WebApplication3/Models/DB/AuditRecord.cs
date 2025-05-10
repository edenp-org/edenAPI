using FreeSql.DataAnnotations;

namespace WebApplication3.Models.DB;

public class AuditRecord
{

    [Column(IsIdentity = true, IsPrimary = true)]
    public int Id { get; set; }

    public int WorkCode { get; set; }

    public string Result { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}