using KnittingApp.Parts;
using KnittingApp.Services;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Model>> GetModels(Guid userId)
        {
            return await modelService.GetModelsByUser(userId);
        }

        public async Task<List<Form>> GetForms(Guid userId)
        {
            return await formService.GetFormsByUser(userId);
        }

        public async Task<Dictionary<string, double>> ChooseForm(Guid formId, Guid userId)
        {
            var form = await formService.GetForm(formId);
            KnittingModelBuilder modelBuilder = new KnittingModelBuilder();
            var model = modelBuilder.CreateModel(form.Name, form.Parts, userId);
            return model.GetMeasures();
        }


        public async Task<Model> GetModel(Guid modelId)
        {
            var model=await modelService.GetModel(modelId);
            if (model == null)
            {
                throw new NullReferenceException("Модель не выбарана");
            }
            return model;
        }


        Model CreateNewModel(string name, List<string> stringParts, Guid userId)
        {
            KnittingModelBuilder modelBuilder = new KnittingModelBuilder();
            var model= modelBuilder.CreateModel(name, stringParts, userId);
            return model;
        }

        public async Task<Form> CreateNewForm(Guid userId, string name, List<string> stringParts)
        {
            var form=await formService.CreateForm(name, userId, stringParts);
            return form;
        }


        void InitializeModel(Model model,Dictionary<string, double> measures,
          double height, double width, int loopInHeight, int loopInWidth)
        {
            if (model == null)
                throw new NullReferenceException("Модель не выбрана");
            double[] loopSize = CalculateLoopSize(height, width, loopInHeight, loopInWidth);
            double loopHeight = loopSize[0];
            double loopWidth = loopSize[1];
            double loopInOneHeight = loopSize[2];
            double loopInOneWidth = loopSize[3];
            model.InitializeParts(measures, loopWidth, loopHeight, loopInOneWidth, loopInOneHeight);
        }


        public async Task<Dictionary<string, double>> GetAllMeasures(Guid modelId)
        {
            var model = await modelService.GetModel(modelId);
            if (model == null)
                throw new NullReferenceException("Модель не выбрана");
            return model.GetMeasures();
        }


        public async Task ChangeMeasure(Guid modelId,string mKey, double mValue)
        {
            await modelService.ChangeMeasure(mKey, mValue);
        }

        public Model CopyModel(int modelIndex)
        {
            return new Model("copy", Guid.Empty);
        }

        public async Task<Draft> CreateDrafts(Guid modelId)
        {
            return await modelService.CreateDrafts(modelId);
        }


        public async Task<Model> CreateModelWithDrafts(Guid formId,Guid userId, string modelName,
            Dictionary<string, double> measures,
          double height, double width, int loopInHeight, int loopInWidth
            )  //возвращает frontDraft
        {
            var form=await formService.GetForm(formId);
            var model = CreateNewModel(modelName, form.Parts, userId);
            InitializeModel(model, measures, height, width, loopInHeight, loopInWidth);
            model.CreateDrafts();
            await modelService.CreateModel(model);
            return model;
        }

        Draft CreateDrafts(Model model)
        {
            model.CreateDrafts();
            return model.frontDraft;
        }


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

        /*public async Task<Draft> MovePoint(Guid Id,Point movingPoint, double newX, double newY, Point leftPoint, Point rightPoint,
            double loopWidth, double loopHeight, double loopInWidth, double loopInHeight)
        {
            *//*return await draftService.MovePoint(draftId, movingPoint, newX, newY, leftPoint, rightPoint,
                loopWidth, loopHeight, loopInWidth, loopInHeight);*/

       // }


        public async Task<Draft> MovePoint(Guid modelId,Point movingPoint, double newX, double newY, Point leftPoint, Point rightPoint, string draftType)
        {
            return await modelService.MovePoint(modelId,movingPoint, newX, newY, leftPoint, rightPoint, draftType);
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
