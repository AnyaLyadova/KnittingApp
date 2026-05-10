using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KnittingApp.Models
{
    public class LoopMapModel
    {
        [Key]
        public Guid LoopMapId {  get; set; }
        [Required]
        [Column(TypeName = "jsonb")]
        public string loopMapJson { get; set; } //хранение матрицы в виде Json

    }
}
