using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace KnittingApp.Models
{
    [Index(nameof(SchemaName), nameof(UserId), IsUnique = true)]
    public class SchemaModel
    {
        [Key]
        public Guid SchemaId { get; set; }
        [Required]
        public string SchemaName { get; set; }
        [Required]
        public string SchemaImage { get; set; }
        [Required]
        public Guid LoopMapId { get; set; }
        [Required]
        public Guid UserId { get; set; }

        [NotMapped]
        public LoopMap LoopMap { get; set; }
        User User;
    }
}
