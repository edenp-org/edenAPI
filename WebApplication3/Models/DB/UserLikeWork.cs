namespace WebApplication3.Models.DB;

public class UserLikeWork
{
    public long WorkCode { get; set; }
    public long UserCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}