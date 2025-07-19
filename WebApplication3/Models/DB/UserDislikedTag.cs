using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models.DB
{
    public class UserDislikedTag
    {
        [Required]
        public long UserCode { get; set; }

        [Required]
        public long TagCode { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string TagName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
