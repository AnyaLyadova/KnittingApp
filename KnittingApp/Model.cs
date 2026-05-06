using KnittingApp.Parts;
using System.Globalization;
using static KnittingApp.SharedConstants;

namespace KnittingApp
{
    public class Model
    {
        string userId;
        public Guid modelId { get;  }
        string name;
        /*Draft FrontNeckDraft;
        Draft BackNeckDraft;
        Draft armholeDraft;
        Draft shoulderDraft;
        Draft frontBodyDraft;
        Draft backBodyDraft;
        Draft sleeveDraft;*/

        List<Part> parts=new List<Part>();  //список частей изделия
       // List <Draft> partDrafts=new List<Draft>(); //список выкроек частей
        Draft frontDraft; //общая выкройка переда
        Draft backDraft; //общая выкройка спинки
        Draft sleeveDraft; //общая выкройка рукава
        Dictionary<string, double> measures = new Dictionary<string, double>();
        bool hasShoulderBevel;   //есть ли скос плеча
        bool hasArmhole;  //есть ли вырез под втачной рукав
        public LoopMap frontLoopMap { get; set; }
        public LoopMap backLoopMap { get; set; }
        public LoopMap sleeveLoopMap { get; set; }

        double loopWidth;  //ширина петли
        double loopHeight;  //высота петли
        double loopInHeight;  //петель в см высоты
        double loopInWidth;  //петель в см ширины
        

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

        public Draft GetFrontDraft()
        {
            return frontDraft;
        }

        public Draft GetBackDraft()
        {
            return backDraft;
        }

        public Draft GetSleeveDraft()
        {
            return sleeveDraft;
        }

        public Draft CreateDrafts()
        {
            CreateFrontDraft();
            CreateBackDraft();
            CreateSleeveDraft();
            return frontDraft;
        }
        public Draft CreateFrontDraft()
        {
            // partDrafts.Clear(); //сбрасываем  состояние
            List<Draft> partDrafts = new List<Draft>();
            Point startPoint = new Point(0, 0, BodyName);
            foreach (var part in parts)
            {
                if (part == null)
                {
                    throw new ArgumentNullException("Переданная часть равна null" + part.GetType());
                }
                if (part.GetPriority() == PartPriority.Sleeve || part.GetPriority() == PartPriority.SleeveRoll)  //пропускаем рукав
                    continue;
                // part.InitializePart(measures, loopWidth, loopHeight);
                Draft draft = part.GetDraft(startPoint);
                partDrafts.Add(draft);
                startPoint = draft.EndPoint;
            }
            //draft.CreateDraft(partDrafts);
            frontDraft = new Draft(partDrafts);
            frontDraft.EndPoint.X = 0;
            frontDraft.Mirror();

            frontLoopMap=new LoopMap(frontDraft, loopWidth, loopHeight);
            return frontDraft;

        }


        public Draft CreateBackDraft()
        {
            Point startPoint = new Point(0, 0, BodyName);
            //  partDrafts.Clear();
            List<Draft> partDrafts = new List<Draft>();
            foreach (var part in parts)
            {
                if (part == null)
                {
                    throw new ArgumentNullException("Переданная часть равна null" + part.GetType());
                }
                if (part.GetPriority() == PartPriority.Sleeve || part.GetPriority() == PartPriority.SleeveRoll)  //пропускаем рукав
                    continue;
                part.DoBackPart();  //делаем выкройку для спинки
                // part.InitializePart(measures, loopWidth, loopHeight);
                Draft draft = part.GetDraft(startPoint);
                partDrafts.Add(draft);
                startPoint = draft.EndPoint;
            }
            //draft.CreateDraft(partDrafts);
            backDraft = new Draft(partDrafts);

            backDraft.EndPoint.X = 0;
            backDraft.Mirror();

            backLoopMap = new LoopMap(backDraft, loopWidth, loopHeight);
            return backDraft;
        }


        public Draft CreateSleeveDraft(){
            Point startPoint = new Point(0, 0, BodyName);
            //partDrafts.Clear();
            List<Draft> partDrafts = new List<Draft>();
            foreach (var part in parts)
            {
                if (part == null)
                {
                    throw new ArgumentNullException("Переданная часть равна null" + part.GetType());
                }
                if (part.GetPriority() == PartPriority.Sleeve || part.GetPriority() == PartPriority.SleeveRoll)  //берем только части рукава
  
               { 
                    Draft draft = part.GetDraft(startPoint);
                    partDrafts.Add(draft);
                    startPoint = draft.EndPoint;
                }
            }
            //draft.CreateDraft(partDrafts);
            sleeveDraft = new Draft(partDrafts);
            sleeveDraft.Mirror();

            sleeveLoopMap = new LoopMap(sleeveDraft, loopWidth, loopHeight);
            return sleeveDraft;
        }


        public void InitializeParts(Dictionary<string, double> addMeasures,
            double loopWidth, double loopHeight,
            double loopInWidth, double loopInHeight)
        {
            if(loopWidth == 0 || loopHeight == 0)
                throw new ArgumentNullException("Переданная плотность вязания равна 0");
            this.loopWidth = loopWidth;
            this.loopHeight = loopHeight;
            this.loopInWidth = loopInWidth;
            this.loopInHeight = loopInHeight;
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
            double upperWidth = (parts[0] as Body).BottomWidth/2 - (parts[0] as Body).TopWidth/2;
            for(int i=1; i<parts.Count-1;++i)
            {
                upperWidth += parts[i].Width;
            }
            upperWidth += parts[Parts.Count - 1].Width / 2;
            if (upperWidth < (parts[0] as Body).BottomWidth)
                (parts[parts.Count-1] as Neck).ChangeWidth((parts[0] as Body).BottomWidth/2 - upperWidth);
        }


        public Draft MovePoint(Point movingPoint, double newX, double newY, Point leftPoint, Point rightPoint, string draftType)
        {
            Draft currentDraft;
            LoopMap currentLoopMap;
            switch (draftType)
            {
                case "front":
                    currentDraft = frontDraft;
                    currentLoopMap = frontLoopMap;
                    break;
                case "back":
                    currentDraft = backDraft;
                    currentLoopMap = backLoopMap;
                    break;
                case "sleeve":
                    currentDraft = sleeveDraft;
                    currentLoopMap = sleeveLoopMap;
                    break;
                default:
                    throw new ArgumentException("Передана некорректная часть чертежа");

            }
            List<Point> newPoints;
            currentDraft.MovePoint(movingPoint, newX, newY,leftPoint,rightPoint, loopWidth,
             loopHeight, loopInWidth,  loopInHeight, out newPoints);
            var leftX=leftPoint.X<newX?leftPoint.X:newX;
            var rightX=rightPoint.X<newX?newX:rightPoint.X;
            var topY = leftPoint.Y;

            if(leftPoint.Y < newY) 
                topY = newY;
            if (rightPoint.Y > newY)
                topY = rightPoint.Y;
            var lowY=leftPoint.Y;
            if(leftPoint.Y >newY)
                lowY = newY;
            if (rightPoint.Y < newY)
                lowY = rightPoint.Y;
            
            if(leftX>newX)
                leftX = newX;
            if (rightX<newX)
                rightX = newX;

            currentLoopMap.RebuildLoopMap(leftX, rightX, topY, lowY, newPoints);
            return currentDraft;
        }

        public LoopMap ColorLoopMap(List<int> mIndexes, List<int>nIndexes, List<string> colors, string draftType)
        {
            LoopMap currentLoopMap;
            switch (draftType)
            {
                case "front":
                    currentLoopMap = frontLoopMap;
                    break;
                case "back":
                    currentLoopMap = backLoopMap;
                    break;
                case "sleeve":
                    currentLoopMap = sleeveLoopMap;
                    break;
                default:
                    throw new ArgumentException("Передана некорректная часть чертежа");

            }
            for (int i=0; i<mIndexes.Count; ++i)
            {
                currentLoopMap.ChangeColor(mIndexes[i],nIndexes[i], colors[i]);
            }
            return currentLoopMap;
        }


        public LoopMap ColorAllLoopMap(string color, string draftType)
        {
            LoopMap currentLoopMap;
            switch (draftType)
            {
                case "front":
                    currentLoopMap = frontLoopMap;
                    break;
                case "back":
                    currentLoopMap = backLoopMap;
                    break;
                case "sleeve":
                    currentLoopMap = sleeveLoopMap;
                    break;
                default:
                    throw new ArgumentException("Передана некорректная часть чертежа");

            }
            currentLoopMap.ColorLoopMap(color);
            return currentLoopMap;
        }

       

    }
}
