using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models.DB
{
    public class UserFavoriteTag
    {
        [Required]
        public string UserCode { get; set; }
        [Required]
        public string TagCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
