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

        public int nullM { get; set; }   //координаты точки 0,0 относительно матрицы
        
        public int nullN { get; set; }
        
        public double loopWidth { get; set; }
        
        public double loopHeight { get; set; }

        public Guid? LoopsReaderId { get; set; }
        [Column(TypeName = "jsonb")]
        public string? NullPoint { get; set; }  

        public LoopsReaderModel? LoopsReader { get; set; }

    }
}
