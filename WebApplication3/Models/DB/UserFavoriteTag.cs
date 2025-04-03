using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models.DB
{
    public class UserFavoriteTag
    {
        [Required]
        public long UserCode { get; set; }
        [Required]
        public long TagCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
