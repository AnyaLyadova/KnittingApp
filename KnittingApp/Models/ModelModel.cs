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

        [NotMapped]
        public Dictionary<string, double> Measures
        {
            get => JsonSerializer.Deserialize<Dictionary<string, double>>(MeasuresJson)
                   ?? new Dictionary<string, double>();
            set => MeasuresJson = JsonSerializer.Serialize(value ?? new Dictionary<string, double>());
        }

        public User User { get; set; }
        public Draft FrontDraft { get; set; }
        public Draft BackDraft { get; set; }
        public Draft SleeveDraft { get; set; }
        public LoopMap FrontLoopMap { get; set; }
        public LoopMap BackLoopMap { get; set; }
        public LoopMap SleeveLoopMap { get; set; }
    }
}
