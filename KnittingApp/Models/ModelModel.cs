using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
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
        [Required]
        public string MeasuresJson { get; set; } = "{}";  //мерки в json
        [Required]
        public double loopWidth { get; set; }
        [Required]
        public double loopHeight { get; set; }  
        [Required]
        public double loopInHeight { get; set; }
        [Required]
        public double loopInWidth { get; set; }

        [NotMapped]
        public Dictionary<string, double> Measures
        {
            get => JsonSerializer.Deserialize<Dictionary<string, double>>(MeasuresJson)
                   ?? new Dictionary<string, double>();
            set => MeasuresJson = JsonSerializer.Serialize(value ?? new Dictionary<string, double>());
        }

        public UserModel User { get; set; }
        public DraftModel FrontDraft { get; set; }
        public DraftModel BackDraft { get; set; }
        public DraftModel SleeveDraft { get; set; }
        public LoopMapModel FrontLoopMap { get; set; }
        public LoopMapModel BackLoopMap { get; set; }
        public LoopMapModel SleeveLoopMap { get; set; }
    }
}
