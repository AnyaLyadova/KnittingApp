using KnittingApp.Parts;
using static KnittingApp.SharedConstants;

namespace KnittingApp
{
    public class Constructor
    {
        List<Model> models= new List<Model>{ new ModelBuilder().CreateModel("new",new List<string> { ArmholeName, ONeckName}) };
        Model model;


        public List<Model> GetModels()
        {
            return models;
        }

        public Model ChooseModel(Guid modelId)
        {
            this.model = null;  //обнуляем состояние
            var model = models.Where(m=>m.modelId==modelId).FirstOrDefault();
            this.model = model;
            if (model == null)
                throw new NullReferenceException("Модели с таким индексом не существует");
            return model;
        }

        public Model GetModel()
        {
            if (model == null)
            {
                throw new NullReferenceException("Модель не выбарана");
            }
            return model;
        }

        public Model CreateNewModel(string name, List<string> stringParts/*, Dictionary<string, double> measures,*/
           /* double height, double width, int loopInHeight, int loopInWidth*/)
        {
            /*double[] loopSize = CalculateLoopSize(height, width, loopInHeight, loopInWidth);
            double loopHeight=loopSize[0];
            double loopWidth=loopSize[1];*/
            ModelBuilder modelBuilder = new ModelBuilder();
            var model= modelBuilder.CreateModel(name, stringParts/*, loopHeight, loopInWidth*//*, measures*/);
            models.Add(model);
            return model;
        }

        public void InitializeModel(Dictionary<string, double> measures,
            double height, double width, int loopInHeight, int loopInWidth)
        {
            if (model == null)
                throw new NullReferenceException("Модель не выбрана");
            double[] loopSize = CalculateLoopSize(height, width, loopInHeight, loopInWidth);
            double loopHeight = loopSize[0];
            double loopWidth = loopSize[1];
            double loopInOneHeight= loopSize[2];
            double loopInOneWidth= loopSize[3];
            model.InitializeParts(measures,loopWidth, loopHeight, loopInOneWidth, loopInOneHeight);
        }

        public Dictionary<string, double> GetAllMeasures()
        {
            return model.GetMeasures();
        }

        public void ChangeMeasure(string mKey, double mValue)
        {
            model.ChangeMeasure(mKey, mValue);
        }

       /* public Draft GetBaseDraft(List<string> stringParts, Dictionary<string, double> measures)
        {
            model=new Model(stringParts, measures);
            return model.CreateDraft();
        }
*/
        public Draft RecalculateDraft(Point oldPoint, Point newPoint)
        {
            return model.RebuildDraft(oldPoint, newPoint);
        }

        public Model CopyModel(int modelIndex)
        {
            return model;
        }


        public Draft GetDraft()
        {
            if (model == null)
                throw new ArgumentNullException("Модель не создана");
            return model.CreateDraft();
        }

        double[] CalculateLoopSize(double height, double width, int loopInHeight, int loopInWidth)
        {
            double loopInOneWidth=loopInWidth/width;
            double loopInOneHeight=loopInHeight/height;
            double loopWidth = width/loopInWidth ;
            double loopHeight = height/loopInHeight;
            double[] loopSize = {loopHeight,loopWidth, loopInOneHeight, loopInOneWidth};
            return loopSize;

        }

        public Dictionary<string, string> GetNeckParts()
        {
            return NeckParts;
        }

        public Dictionary<string, string> GetArmholeParts()
        {
            return ArmholeParts;
        }

        public Dictionary<string, string> GetSleeveRollParts()
        {
            return SleeveRollParts;
        }
        
        public LoopMap GetLoopMap()
        {
            if (model.loopMap == null)
                throw new NullReferenceException("Матрица петель не инициалзирована");
            return model.loopMap;
        }


        public Draft MovePoint(Point movingPoint, double newX, double newY, Point leftPoint, Point rightPoint)
        {
            return model.MovePoint(movingPoint, newX, newY,leftPoint, rightPoint);
        }

        public LoopMap ColorLoopMap(List<int> mIndexes, List<int> nIndexes, List<string> colors)
        {
           return model.ColorLoopMap(mIndexes, nIndexes, colors);

        }

    }

}
