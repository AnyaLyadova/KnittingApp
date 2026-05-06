using KnittingApp.Extensions;
using KnittingApp.Models;
using KnittingApp.Repository;
using KnittingApp.Extensions;
using static KnittingApp.SharedConstants;
using System.Threading.Tasks;
namespace KnittingApp.Services
{
    public class LoopMapService
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

        public async Task DraftToLoopMap(Draft draft,double loopWidth, double loopHeight)  //конвертация выкройки в матрицу петель
        {
            Point upper = draft.GetUpperPoint();
            Point bottom = draft.GetBottomPoint();
            int m = (int)(Math.Abs(upper.Y - bottom.Y) / loopHeight) + AddingM + 1;
            int n = (int)(Math.Abs(upper.X - bottom.X) / loopWidth) + AddingN + 1;
            var loopMap = new LoopMap(m, n, "draft");

            // Point nullPoint=draft.StartPoint;  //точка (0,0) в начале чертежа (середина низа)
            Point nullPoint = draft.GetUpperPoint(); // точка (0,0) левый верхний угол
            int nullM = (int)((nullPoint.Y - draft.StartPoint.Y) / loopHeight) + AddingM / 2;
            int nullN = (int)((draft.StartPoint.X - nullPoint.X) / loopWidth) + AddingN / 2;
            //   nullPoint.X += AddingN / 2;  //смещаем точку для дальнейшего расширения
            //  nullPoint.Y += AddingM / 2;

            // AddLoop(0, 0, LoopType.loop, LoopSide.front);
            int count = 1;
            foreach (var point in draft.draft)  //переносим контур выкройки в матрицу относительно нуля
            {
                if (point.Equals(draft.StartPoint) || point.Equals(draft.EndPoint)) //пропускаем начальную и конечную точки, так как они в центре
                    continue;
                int mLoop = (int)((nullPoint.Y - point.Y) / loopHeight) + AddingM / 2;
                int nLoop = (int)((point.X - nullPoint.X) / loopWidth) + AddingN / 2;
                LoopSide side;
                if (count % 2 != 0)
                    side = LoopSide.front;
                else
                    side = LoopSide.back;
                loopMap.AddLoop(mLoop, nLoop, LoopType.loop, side);
                ++count;
            }
            for (int i = loopMap.loopMap.Length - 1; i >= 0; i--)  //заполняем контур снизу вверх 
            {
                bool isOpen = false;  //маркер нахождения внутри контура
                LoopType type = LoopType.none;
                for (int j = loopMap.loopMap[0].Length - 1; j >= 0; j--)  //справа налево
                {
                    LoopSide side;
                    if (i % 2 != 0)  //нечетные ряды лицевые
                        side = LoopSide.front;
                    else
                        side = LoopSide.back;  //четные изнаночные
                    if (loopMap.loopMap[i][j] != null && loopMap.loopMap[i][j].GetType() == LoopType.loop)  //встречаем контур
                    {
                        if (isOpen)  //контур закрылся, заканчиваем заполнение
                        {
                            isOpen = false;
                            type = LoopType.none;
                        }
                        else  //дошли до контура, начинаем заполнение
                        {
                            isOpen = true;
                            type = LoopType.loop;
                        }
                    }
                    if (isOpen)
                    {
                        loopMap.AddLoop(i, j, type, side);  //добавляем петлю нужного типа и вида
                    }

                }
            }

            for (int i = 1; i < loopMap.loopMap.Length - 1; ++i)  //проставляем убавки и прибавки сверху вниз
            {
                for (int j = 0; j < loopMap.loopMap[i].Length; ++j)  //слева направо
                {
                    if (loopMap.loopMap[i][j] != null)
                    {
                        if ((loopMap.loopMap[i][j].GetType() == LoopType.loop
                        && loopMap.loopMap[i + 1][j] == null) ||
                        (loopMap.loopMap[i][j].GetType() == LoopType.loop
                        && loopMap.loopMap[i + 1][j].GetType() == LoopType.none))  //убавка
                        {
                            loopMap.loopMap[i][j].type = LoopType.decrease;
                        }
                        if ((loopMap.loopMap[i][j].GetType() == LoopType.loop
                            && loopMap.loopMap[i - 1][j] == null) ||
                            (loopMap.loopMap[i][j].GetType() == LoopType.loop
                            && loopMap.loopMap[i - 1][j].GetType() == LoopType.none))  //прибавкуа
                        {
                            loopMap.loopMap[i][j].type = LoopType.increase;
                        }
                    }
                }
            }
            await loopMapRepository.CreateLoopMap(loopMap.ToModel());
        }


        public void RebuildLoopMap(double leftX, double rightX, double topY, double lowY, List<Point> newPoints)
        {
            int lowM = (int)((nullPoint.Y - lowY) / loopHeight) + AddingM / 2; ;
            int topM = (int)((nullPoint.Y - topY) / loopHeight) + AddingM / 2; ;
            int rightN = (int)((rightX - nullPoint.X) / loopWidth) + AddingN / 2;
            int leftN = (int)((leftX - nullPoint.X) / loopWidth) + AddingN / 2;

            for (int i = lowM; i >= topM; --i)  //обнуляем старые петли
            {
                for (int j = leftN; j <= rightN; ++j)
                {
                    loopMap[i][j] = null;
                }
            }

            int count = 1;
            foreach (var point in newPoints)  //переносим новые точки в матрицу относительно нуля
            {

                int m = (int)((nullPoint.Y - point.Y) / loopHeight) + AddingM / 2;
                int n = (int)((point.X - nullPoint.X) / loopWidth) + AddingN / 2;
                LoopSide side;
                if (count % 2 != 0)
                    side = LoopSide.front;
                else
                    side = LoopSide.back;
                AddLoop(m, n, LoopType.loop, side);
                ++count;
            }



            for (int i = lowM; i >= topM; i--)  //заполняем контур снизу вверх в измененном диапазоне
            {
                int loopCount = 0;
                for (int h = rightN + 1; h >= leftN - 1; h--)
                {
                    if (loopMap[i][h] != null && loopMap[i][h].GetType() == LoopType.loop)
                        ++loopCount;
                }
                bool isOpen = false;  //маркер нахождения внутри контура
                LoopType type = LoopType.none;
                for (int j = rightN + 1; j >= leftN - 1; j--)  //справа налево
                {
                    LoopSide side;
                    if (i % 2 != 0)  //нечетные ряды лицевые
                        side = LoopSide.front;
                    else
                        side = LoopSide.back;  //четные изнаночные
                    if (loopMap[i][j] != null && loopMap[i][j].GetType() == LoopType.loop && loopCount > 1 && loopCount <= 2)  //встречаем контур
                    {
                        if (isOpen)  //контур закрылся, заканчиваем заполнение
                        {
                            isOpen = false;
                            type = LoopType.none;
                        }
                        else  //дошли до контура, начинаем заполнение
                        {
                            isOpen = true;
                            type = LoopType.loop;
                        }
                    }
                    if (isOpen)
                    {
                        AddLoop(i, j, type, side);  //добавляем петлю нужного типа и вида
                    }

                }
            }

            for (int i = topM; i <= lowM; ++i)  //проставляем убавки и прибавки сверху вниз
            {
                for (int j = leftN; j < rightN; ++j)  //слева направо
                {
                    if (loopMap[i][j] != null)
                    {
                        if ((loopMap[i][j].GetType() == LoopType.loop
                        && loopMap[i + 1][j] == null) ||
                        (loopMap[i][j].GetType() == LoopType.loop
                        && loopMap[i + 1][j].GetType() == LoopType.none))  //убавка
                        {
                            loopMap[i][j].type = LoopType.decrease;
                        }
                        if ((loopMap[i][j].GetType() == LoopType.loop
                            && loopMap[i - 1][j] == null) ||
                            (loopMap[i][j].GetType() == LoopType.loop
                            && loopMap[i - 1][j].GetType() == LoopType.none))  //прибавка
                        {
                            loopMap[i][j].type = LoopType.increase;
                        }
                    }
                }
            }

        }

    }
}
