using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models.DB
{
    public class UserDislikedTag
    {
        [Required]
        [StringLength(50)]
        public long UserCode { get; set; }

        [Required]
        [StringLength(50)]
        public long TagCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
