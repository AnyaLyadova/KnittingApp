using System.Text.Json.Serialization;
using static KnittingApp.SharedConstants;
namespace KnittingApp
{
    public class Loop
    {
        string color;

        [JsonPropertyName("color")]
        public string Color { get { return color; } set { color = value; } } 
        public LoopType type { get; set; }
        public LoopSide side { get; set; }

        public Loop()
        {
            type = LoopType.none;
            side=LoopSide.front;
            color = "none";
        }
        public Loop(LoopType type, LoopSide side)
        {
            this.type = type;
            this.side = side;
            color = "none";
        }

        public LoopType GetType()
        {
            return type;
        }

        public LoopSide GetSide()
        {
            return side;
        }
        
    }
}


