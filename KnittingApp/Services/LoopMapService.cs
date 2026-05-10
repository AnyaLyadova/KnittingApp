using KnittingApp.Extensions;
using KnittingApp.Models;
using KnittingApp.Repository;
using KnittingApp.Extensions;
using static KnittingApp.SharedConstants;
using System.Threading.Tasks;
namespace KnittingApp.Services
{
    public class LoopMapService:ILoopMapService
    {

        private readonly ILoopMapRepository loopMapRepository;

        public LoopMapService(ILoopMapRepository loopMapRepository)
        {
            this.loopMapRepository=loopMapRepository;
        }

        /* public void AddPattern(int x, int y, LoopMap pattern)
        {
            if (pattern == null) throw new ArgumentNullException();
            for(int i = x; i<=pattern.loopMap.Length; ++i)
            {
                for(int j=y; j <= pattern.loopMap[i].Length; ++j)
                {
                    loopMap[i][j]=pattern.loopMap[i][j];
                }
            }
        }*/


        public async Task ColorLoopMap(Guid loopMapId,string color)
        {
            var loopMapModel=await loopMapRepository.GetLoopMap(loopMapId);
            LoopMap loopMap = loopMapModel.ToObject();
            if (loopMap == null) throw new ArgumentNullException("Схема не инициализирована");
            for (int i = 0; i < loopMap.loopMap.Length; ++i)
            {
                for (int j = 0; j < loopMap.loopMap[i].Length; ++j)
                {
                    if (loopMap.loopMap[i][j] != null && loopMap.loopMap[i][j].GetType() != LoopType.none)
                        loopMap.loopMap[i][j].Color = color;
                }
            }
            await loopMapRepository.UpdateLoopMap(loopMap.ToModel());
        }
        public async Task AddLoop(Guid loopMapId,int m, int n, LoopType type, LoopSide side)
        {
            var loopMapModel = await loopMapRepository.GetLoopMap(loopMapId);
            LoopMap loopMap= loopMapModel.ToObject();
            loopMap.loopMap[m][n] = new Loop(type, side);
            await loopMapRepository.UpdateLoopMap(loopMap.ToModel());
        }

        public async Task ChangeColor(Guid loopMapId,int m, int n, string color)
        {
            var loopMapModel = await loopMapRepository.GetLoopMap(loopMapId);
            LoopMap loopMap = loopMapModel.ToObject();
            if (loopMap.loopMap[m][n] != null && loopMap.loopMap[m][n].GetType() != LoopType.none)
                loopMap.loopMap[m][n].Color = color;
            await loopMapRepository.UpdateLoopMap(loopMap.ToModel());
        }

        public async Task ChangeType(Guid loopMapId,int m, int n, LoopType type)
        {
            var loopMapModel = await loopMapRepository.GetLoopMap(loopMapId);
            LoopMap loopMap = loopMapModel.ToObject();
            loopMap.loopMap[m][n].type = type;
            await loopMapRepository.UpdateLoopMap(loopMap.ToModel());
        }

        public async Task <LoopMap> CreateLoopMapByDraft(Draft draft,double loopWidth, double loopHeight)  //конвертация выкройки в матрицу петель
        {
            var loopMap=new LoopMap(draft, loopWidth,loopHeight);
            await loopMapRepository.CreateLoopMap(loopMap.ToModel());
            return loopMap;
        }


        public async Task RebuildLoopMap(Guid id,double leftX, double rightX, double topY, double lowY, List<Point> newPoints)
        {
            var loopMapModel=await loopMapRepository.GetLoopMap(id);
            var loopMap = loopMapModel.ToObject();
            loopMap.RebuildLoopMap(leftX, rightX, topY, lowY, newPoints);
            await loopMapRepository.UpdateLoopMap(loopMap.ToModel());

        }

    }
}
