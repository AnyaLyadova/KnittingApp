using static KnittingApp.SharedConstants;
namespace KnittingApp
{
    public class Schema
    {
        public Guid schemaId { get; set; }
        public int m;  //ширина
        public int n;  //высота
        public string schemaName { get; set; }

        public LoopMap loopMap { get; }

        public Schema(int m, int n, string schemaName)
        {
            this.m = m;
            this.n = n;
            this.schemaName = schemaName;
            loopMap = new LoopMap(m, n);
            schemaId = Guid.NewGuid();
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
