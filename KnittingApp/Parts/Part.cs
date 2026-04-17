using static KnittingApp.SharedConstants;
namespace KnittingApp.Parts
{
    public class Part  //базовый класс для частей изделия
    {
        public string name;
        /*protected double width;
        protected double height;*/
        protected double loopWidth; //плотность вязания в ширину (ширина петли)
        protected double loopHeight; //плотность вязания в высоту (высота ряда)
        protected Dictionary<string, double> partMeasures;
        protected bool isFront = true;
        protected PartPriority priority;
        protected double loopInWidth; //кол-во петель в 1 см в ширину
        protected double loopInHeight; //кол-во петель в 1 см в длину

        public double Width { get; protected set; }
        public void DoBackPart()
        {
            isFront = false;
        }

        public PartPriority GetPriority()
        {
            return priority;
        }
        public Dictionary<string, double> PartMeasures { get { return partMeasures; } }

        /* public Part(string name, double width, double height, int loopWidth, int loopHeight)
         {
             this.name = name;
             this.width = width;
             this.height = height;
             this.loopWidth=loopWidth;
             this.loopHeight=loopHeight;
         }*/

        public Part(string name, PartPriority priority)
        {
            this.name = name;
            this.priority = priority;
        }

        public virtual void InitializePart(Dictionary<string,double> measures, double loopWidth, double loopHeight
            , double loopInWidth, double loopInHeight)  //инициализация значениями части
        {
            this.loopWidth = loopWidth;
            this.loopHeight = loopHeight;
            this.loopInWidth = loopInWidth;
            this.loopInHeight = loopInHeight;
            foreach(var measure in partMeasures.Keys.ToList())
            {
                if (measures.TryGetValue(measure, out double value)){
                   partMeasures[measure] = value;
                }
                else
                {
                    throw new InvalidDataException("Необходимая мерка отсутствует в списке мерок");
                }
            }
        }
        public virtual Draft GetDraft(Point startPoint)  //метод расчета и составления выкройки
        {
            return new Draft(startPoint);
        }
    }
}
