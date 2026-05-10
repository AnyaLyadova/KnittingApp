using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace KnittingApp.Models
{
    public class LoopsReaderModel
    {
        [Key]
        public Guid LoopReaderId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public int CurrentIndex { get; set; }
        [Required]
        public Guid LoopMapId { get; set; }
        [Required]
        public TimeSpan SpentTime { get; set; }

        [NotMapped]
        public LoopMap LoopMap { get; set; }
    }
}
