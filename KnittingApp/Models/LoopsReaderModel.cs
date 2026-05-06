using System.ComponentModel.DataAnnotations;
namespace KnittingApp.Models
{
    public class LoopsReaderModel
    {
        [Key]
        public Guid LoopReaderId { get; set; }
        [Required]
        public int CurrentIndex { get; set; }
        [Required]
        public Guid LoopMapId { get; set; }
        [Required]
        public TimeSpan SpentTime { get; set; }

        LoopMap LoopMap;
    }
}
