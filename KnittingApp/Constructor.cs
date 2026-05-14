using KnittingApp.Parts;
using KnittingApp.Services;
using System.Threading.Tasks;
using static KnittingApp.SharedConstants;

namespace KnittingApp
{
    public class Constructor
    {
        private readonly IModelService modelService;
        private readonly IDraftService draftService;
        private readonly IFormService formService;
        private readonly ILoopMapService loopMapService;

        public Constructor(IModelService modelService, IDraftService draftService, IFormService formService, ILoopMapService loopMapService)
        {
            this.modelService = modelService;
            this.draftService = draftService;
            this.formService = formService;
            this.loopMapService = loopMapService;
        }

        // List<Model> models= new List<Model>{ new ModelBuilder().CreateModel("new",new List<string> { ArmholeName, ONeckName}) };
        //  Model model;
        // List<Form> forms = new List<Form>() { new Form("new", Guid.NewGuid(),new List<string> { ArmholeName, ONeckName })};


        /*public List<Model> GetModels()
        {
            return models;
        }*/

        public async Task<List<Model>> GetModels(Guid userId)
        {
            return await modelService.GetModelsByUser(userId);
        }

        public async Task<List<Form>> GetForms(Guid userId)
        {
            return await formService.GetFormsByUser(userId);
        }

        /*public Model ChooseModel(Guid modelId)
        {
            this.model = null;  //обнуляем состояние
            var model = models.Where(m=>m.modelId==modelId).FirstOrDefault();
            this.model = model;
            if (model == null)
                throw new NullReferenceException("Модели с таким индексом не существует");
            return model;
        }*/

        /*public Model ChooseForm(Guid formId)
        {
            var form=forms.Where(f=>f.formId==formId).FirstOrDefault();
            var model = CreateNewModel(form.Name, form.Parts);
            return model;
        }*/

        public async Task<Model> ChooseForm(Guid formId)
        {
            var form = await formService.GetForm(formId);
            var model = CreateNewModel(form.Name, form.Parts);
            return model;
        }

        /*public Model GetModel()
        {
            if (model == null)
            {
                throw new NullReferenceException("Модель не выбарана");
            }
            return model;
        }*/

        public async Task<Model> GetModel(Guid modelId)
        {
            var model=await modelService.GetModel(modelId);
            if (model == null)
            {
                throw new NullReferenceException("Модель не выбарана");
            }
            return model;
        }

       /* public Model GetModel(Guid modelId)
        {
            var model=models.Where(m=>m.modelId==modelId).FirstOrDefault();
            if (model == null)
            {
                throw new NullReferenceException("Модель не выбарана");
            }
            return model;
        }*/

        public Model CreateNewModel(string name, List<string> stringParts/*, Dictionary<string, double> measures,*/
           /* double height, double width, int loopInHeight, int loopInWidth*/)
        {
            /*double[] loopSize = CalculateLoopSize(height, width, loopInHeight, loopInWidth);
            double loopHeight=loopSize[0];
            double loopWidth=loopSize[1];*/
            ModelBuilder modelBuilder = new ModelBuilder();
            var model= modelBuilder.CreateModel(name, stringParts/*, loopHeight, loopInWidth*//*, measures*/);
           // models.Add(model);
            return model;
        }

        public async Task<Form> CreateNewForm(Guid userId, string name, List<string> stringParts)
        {
            var form=await formService.CreateForm(name, userId, stringParts);
            return form;
        }

        /*public void InitializeModel(Dictionary<string, double> measures,
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
        }*/


        /*public void InitializeModel(Guid modelId,Dictionary<string, double> measures,
          double height, double width, int loopInHeight, int loopInWidth)
        {
            var model= models.Where(m => m.modelId == modelId).FirstOrDefault();
            if (model == null)
                throw new NullReferenceException("Модель не выбрана");
            double[] loopSize = CalculateLoopSize(height, width, loopInHeight, loopInWidth);
            double loopHeight = loopSize[0];
            double loopWidth = loopSize[1];
            double loopInOneHeight = loopSize[2];
            double loopInOneWidth = loopSize[3];
            model.InitializeParts(measures, loopWidth, loopHeight, loopInOneWidth, loopInOneHeight);
        }*/

        public async Task InitializeModel(Guid modelId, Dictionary<string, double> measures,
          double height, double width, int loopInHeight, int loopInWidth)
        {
            var model = await modelService.GetModel(modelId);
            if (model == null)
                throw new NullReferenceException("Модель не выбрана");
            double[] loopSize = CalculateLoopSize(height, width, loopInHeight, loopInWidth);
            double loopHeight = loopSize[0];
            double loopWidth = loopSize[1];
            double loopInOneHeight = loopSize[2];
            double loopInOneWidth = loopSize[3];
            model.InitializeParts(measures, loopWidth, loopHeight, loopInOneWidth, loopInOneHeight);
            //////////////////////////////////////////
        }

        /*public Dictionary<string, double> GetAllMeasures()
        {
            return model.GetMeasures();
        }*/

        /*public Dictionary<string, double> GetAllMeasures(Guid modelId)
        {
            var model = models.Where(m => m.modelId == modelId).FirstOrDefault();
            if (model == null)
                throw new NullReferenceException("Модель не выбрана");
            return model.GetMeasures();
        }*/

        public async Task<Dictionary<string, double>> GetAllMeasures(Guid modelId)
        {
            var model = await modelService.GetModel(modelId);
            if (model == null)
                throw new NullReferenceException("Модель не выбрана");
            return model.GetMeasures();
        }

        /*public void ChangeMeasure(string mKey, double mValue)
        {
            model.ChangeMeasure(mKey, mValue);
        }*/

        public async Task ChangeMeasure(Guid modelId,string mKey, double mValue)
        {
            await modelService.ChangeMeasure(mKey, mValue);
        }

        /* public Draft GetBaseDraft(List<string> stringParts, Dictionary<string, double> measures)
         {
             model=new Model(stringParts, measures);
             return model.CreateDraft();
         }
 */


        public Model CopyModel(int modelIndex)
        {
            return new Model("copy");
        }


        /*public Draft CreateDrafts()
        {
            if (model == null)
                throw new ArgumentNullException("Модель не создана");
            return model.CreateDrafts();
        }*/

        public async Task<Draft> CreateDrafts(Guid modelId)
        {
            return await modelService.CreateDrafts(modelId);
        }


        /* public Draft GetFrontDraft()
         {
             if (model == null)
                 throw new ArgumentNullException("Модель не создана");
             return model.GetFrontDraft();
         }

         public Draft GetBackDraft()
         {
             if (model == null)
                 throw new ArgumentNullException("Модель не создана");
             return model.GetBackDraft();
         }

         public Draft GetSleeveDraft()
         {
             if (model == null)
                 throw new ArgumentNullException("Модель не создана");
             return model.GetSleeveDraft();
         }*/


        public async Task<Draft> GetFrontDraft(Guid modelId)
        {
            var draft=await modelService.GetFrontDraft(modelId);
            if (draft == null)
                throw new ArgumentNullException("Чертеж не создан");
            return draft;
        }

        public async  Task<Draft> GetBackDraft(Guid modelId)
        {
            var draft = await modelService.GetBackDraft(modelId);
            if (draft == null)
                throw new ArgumentNullException("Чертеж не создан");
            return draft;
        }

        public async Task<Draft> GetSleeveDraft(Guid modelId)
        {
            var draft = await modelService.GetSleeveDraft(modelId);
            if (draft == null)
                throw new ArgumentNullException("Чертеж не создан");
            return draft;
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

        /*public LoopMap GetLoopMap(string draftType)
        {
            LoopMap currentLoopMap;
            switch (draftType)
            {
                case "front":
                    currentLoopMap = model.frontLoopMap;
                    break;
                case "back":;
                    currentLoopMap = model.backLoopMap;
                    break;
                case "sleeve":
                    currentLoopMap = model.sleeveLoopMap;
                    break;
                default:
                    throw new ArgumentException("Передана некорректная часть чертежа");

            }
            if (currentLoopMap == null)
                throw new NullReferenceException("Матрица петель не инициалзирована");
            return currentLoopMap;
        }*/

        public async Task<LoopMap> GetLoopMap(Guid modelId, string draftType)
        {
            LoopMap currentLoopMap;
            switch (draftType)
            {
                case "front":
                    currentLoopMap = await modelService.GetFrontLoopMap(modelId);
                    break;
                case "back":
                    ;
                    currentLoopMap = await modelService.GetBackLoopMap(modelId);
                    break;
                case "sleeve":
                    currentLoopMap = await modelService.GetSleeveLoopMap(modelId);
                    break;
                default:
                    throw new ArgumentException("Передана некорректная часть чертежа");

            }
            if (currentLoopMap == null)
                throw new NullReferenceException("Матрица петель не инициалзирована");
            return currentLoopMap;
        }

        /* public Draft MovePoint(Point movingPoint, double newX, double newY, Point leftPoint, Point rightPoint, string draftType)
         {
             return model.MovePoint(movingPoint, newX, newY,leftPoint, rightPoint, draftType);
         }*/

        public async Task<Draft> MovePoint(Guid draftId,Point movingPoint, double newX, double newY, Point leftPoint, Point rightPoint,
            double loopWidth, double loopHeight, double loopInWidth, double loopInHeight)
        {
            return await draftService.MovePoint(draftId, movingPoint, newX, newY, leftPoint, rightPoint,
                loopWidth, loopHeight, loopInWidth, loopInHeight);
        }


        /*public LoopMap ColorLoopMap(List<int> mIndexes, List<int> nIndexes, List<string> colors, string draftType)
        {
           return model.ColorLoopMap(mIndexes, nIndexes, colors, draftType);

        }*/

        public async Task<LoopMap> ColorLoopMap(Guid modelId, List<int> mIndexes, List<int> nIndexes, List<string> colors, string draftType)
        {
            return await modelService.ColorLoopMap(modelId, mIndexes, nIndexes, colors, draftType);

        }

        /*public LoopMap ColorAllLoopMap(string color, string draftType)
        {
            return model.ColorAllLoopMap(color, draftType);
        }*/

        public async Task<LoopMap> ColorAllLoopMap(Guid modelId, string color, string draftType)
        {
            return await modelService.ColorAllLoopMap(modelId, color, draftType);
        }



    }

}
