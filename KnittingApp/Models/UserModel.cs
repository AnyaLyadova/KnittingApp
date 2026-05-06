using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace KnittingApp.Models
{
    [Index(nameof(Username), IsUnique = true)]
    public class UserModel
    {
        [Key]
        public Guid UserId { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
