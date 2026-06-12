using static KnittingApp.SharedConstants;
namespace KnittingApp
{
    public class Schema
    {
        public Guid userId { get; }
        public Guid schemaId { get; set; }
        public int m;  //ширина
        public int n;  //высота
        public string schemaName { get; set; }
        public Guid LoopMapId { get; }

        public LoopMap loopMap { get; set; }
        public string schemaImage { get; set; }

        public Schema(Guid loopMapId, Guid userId)
        {
            LoopMapId = loopMapId;
            this.userId = userId;
        }
        public Schema(int m, int n, string schemaName, Guid userId)
        {
            this.m = m;
            this.n = n;
            this.schemaName = schemaName;
            loopMap = new LoopMap(m, n);
            LoopMapId=loopMap.LoopMapId;
            loopMap.LoopsReaderId = null;
            schemaId = Guid.NewGuid();
            schemaImage = "null";
            this.userId=userId;
        }

        public void ChangeLoopColor(int m, int n, string color)
        {
            if (loopMap == null)
            {
                throw new NullReferenceException("LoopMap равно null");
            }
            loopMap.ChangeColor(m, n, color);
        }

        public void ChangeLoopType(int m, int n, LoopType type)
        {
            if (loopMap == null)
            {
                throw new NullReferenceException("LoopMap равно null");
            }
            loopMap.ChangeType(m, n, type);
        }

        public void ColorSchema(string color)
        {
            loopMap.ColorLoopMap(color);
        }

    }
}
