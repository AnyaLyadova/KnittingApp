using System.ComponentModel.DataAnnotations;
namespace KnittingApp.Models
{
    public class DraftModel
    {
        [Key]
        public Guid DraftId { get; set; }
        [Required]
        public LinkedList<Point> Draft { get; set; }
        [Required]
        public double EndPointY { get; set; }
    }
}
