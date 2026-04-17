using KnittingApp.Parts;
using System.Globalization;
using static KnittingApp.SharedConstants;

namespace KnittingApp
{
    public class Model
    {
        public Guid modelId { get; set; }
        string name;
        /*Draft FrontNeckDraft;
        Draft BackNeckDraft;
        Draft armholeDraft;
        Draft shoulderDraft;
        Draft frontBodyDraft;
        Draft backBodyDraft;
        Draft sleeveDraft;*/

        List<Part> parts=new List<Part>();  //список частей изделия
        List <Draft> partDrafts=new List<Draft>(); //список выкроек частей
        Draft frontDraft; //общая выкройка переда
        Draft backDraft; //общая выкройка спинки
        Draft sleevwDraft; //общая выкройка рукава
        Dictionary<string, double> measures = new Dictionary<string, double>();
        bool hasShoulderBevel;   //есть ли скос плеча
        bool hasArmhole;  //есть ли вырез под втачной рукав
        LoopMap loopMap;
        double loopWidth;
        double loopHeight;

        public List<Part> Parts { get { return parts; } }

        public Model(string name/*,double loopWidth, double loopHeight*//*, Dictionary<string, double> measures*/)  // задание мерок, инициализация частей
        {
            this.modelId = Guid.NewGuid();
            this.name = name;
            /*this.loopWidth = loopWidth;
            this.loopHeight = loopHeight;*/
           // this.measures = measures;
        }
        public string Name { get { return name; } set { name = value; } }

        public void AddPart(Part part)
        {
            if (part == null)
                throw new ArgumentNullException("Переданная часть равна null");
            parts.Add(part);
        }
        public void AddMeasure(Dictionary<string, double> addMeasure)
        {
           // measures.Union(addMeasure);
           foreach (var m in addMeasure)
            {
                measures.Add(m.Key, m.Value);
            }
        }

        public Draft CreateDraft()
        {
            partDrafts.Clear(); //сбрасываем  состояние
            Point startPoint = new Point(0, 0, BodyName);
            foreach (var part in parts)
            {
                if (part == null)
                {
                    throw new ArgumentNullException("Переданная часть равна null" + part.GetType());
                }
               // part.InitializePart(measures, loopWidth, loopHeight);
                Draft draft = part.GetDraft(startPoint);
                partDrafts.Add(draft);
                startPoint = draft.EndPoint;
            }
            //draft.CreateDraft(partDrafts);
            frontDraft = new Draft(partDrafts);
            frontDraft.Mirror();

            loopMap=new LoopMap(frontDraft, loopWidth, loopHeight);
            return frontDraft;

        }

        public Draft CreateBackDraft()
        {
            Point startPoint = new Point(0, 0, BodyName);
            foreach (var part in parts)
            {
                part.DoBackPart();
                // part.InitializePart(measures, loopWidth, loopHeight);
                Draft draft = part.GetDraft(startPoint);
                partDrafts.Add(draft);
                startPoint = draft.EndPoint;
            }
            //draft.CreateDraft(partDrafts);
            frontDraft = new Draft(partDrafts);
            frontDraft.Mirror();

            loopMap = new LoopMap(frontDraft, loopWidth, loopHeight);
            return frontDraft;
        }

        public Draft RebuildDraft(Point oldPoint, Point newPoint)
        {
            return frontDraft;
        }

        public void InitializeParts(Dictionary<string, double> addMeasures,
            double loopWidth, double loopHeight,
            double loopInWidth, double loopInHeight)
        {
            if(loopWidth == 0 || loopHeight == 0)
                throw new ArgumentNullException("Переданная плотность вязания равна 0");
            this.loopWidth = loopWidth;
            this.loopHeight = loopHeight;
            if (addMeasures == null)
                throw new ArgumentNullException("Переданные мерки равны null");
            if (measures.Count == 0)
                throw new ArgumentNullException("Мерки для модели не заданы");
            foreach(var measure in measures.Keys)
            {
                if (addMeasures.TryGetValue(measure, out double value))
                {
                    if (value == 0)
                    {
                        throw new InvalidDataException("Необходимая мерка "+measure+" не инициализирована (равна 0)");
                    }
                    measures[measure] = value;
                }
                else
                {
                    throw new InvalidDataException("Необходимая мерка "+ measure+" отсутствует в списке мерок");
                }
            }
            foreach (var part in parts)
            {
                part.InitializePart(measures, loopWidth, loopHeight, loopInWidth, loopInHeight);
            }
            CheckWidth();
        }

        public void ChangeMeasure(string mKey, double mValue)
        {
            measures[mKey] = mValue;
        }
       
        public Dictionary<string, double> GetMeasures() { return measures; }
        
        void CheckWidth()
        {
            double upperWidth = (parts[0] as Body).BottomWidth - (parts[0] as Body).TopWidth;
            for(int i=1; i<parts.Count;++i)
            {
                upperWidth += parts[i].Width;
            }
            if (upperWidth < (parts[0] as Body).BottomWidth)
                (parts[parts.Count-1] as Neck).ChangeWidth((parts[0] as Body).BottomWidth - upperWidth);
        }

    }
}
