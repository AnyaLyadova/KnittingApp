using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace KnittingApp.Models
{
    [Index(nameof(ModelName), nameof(UserId), IsUnique = true)]
    public class ModelModel
    {
        [Key]
        public Guid ModelId { get; set; }
        [Required]
        public string ModelName { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid FrontLoopMapId { get; set; }
        [Required]
        public Guid BackLoopMapId { get; set; }
        [Required]
        public Guid SleeveLoopMapId { get; set; }
        [Required]
        public Guid FrontDraftId { get; set; }
        [Required]
        public Guid BackDraftId { get; set; }
        [Required]
        public Guid SleeveDraftId { get; set; }

        public User User;
        public Draft FrontDraft;
        public Draft BackDraft;
        public Draft SleeveDraft;
        public LoopMap FrontLoopMap;
        public LoopMap BackLoopLoopMap;
        public LoopMap SleeveLoopMap;
    }
}
