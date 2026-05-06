using System.ComponentModel.DataAnnotations;

namespace KnittingApp.Models
{
    public class LoopMapModel
    {
        [Key]
        public Guid LoopMapId {  get; set; }
        [Required]
        public Loop[][] loopMap { get; set; }

    }
}
