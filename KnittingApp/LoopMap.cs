using KnittingApp.Models;
using Microsoft.Extensions.Primitives;
using System.Drawing;
using static KnittingApp.SharedConstants;
namespace KnittingApp
{
    public class LoopMap
    {
        public Guid LoopMapId { get; }
        public Loop[][] loopMap {  get; }
        public int m { get; set; }
        public int n { get; set; }
        Point nullPoint;
        public int nullM { get; set; }   //координаты точки 0,0 относительно матрицы
        public int nullN { get; set; }
        public double loopWidth { get; set; }
        public double loopHeight { get; set; }
       /* public LoopMap(Draft draft, double loopWidth, double loopHeight) {
            LoopMapId = new Guid();
            Point upper = draft.GetUpperPoint();
            Point bottom = draft.GetBottomPoint();
            int m = (int)(Math.Abs(upper.Y - bottom.Y)/loopHeight) + AddingM + 1;
            int n = (int)(Math.Abs(upper.X - bottom.X)/loopWidth) + AddingN + 1;
            this.m = m;
            this.n = n;
            this.loopWidth = loopWidth;
            this.loopHeight = loopHeight;
            loopMap = new Loop[m+1][];
            for (int i = 0; i < m+1; i++)
            {
                loopMap[i] = new Loop[n+1];  // все ячейки будут null
            }
            
            DraftToLoopMap(draft);
        }*/

        public LoopMap(int m, int n, string draft)
        {

            this.m = m;
            this.n = n;
            this.loopWidth = loopWidth;
            this.loopHeight = loopHeight;
            loopMap = new Loop[m + 1][];
            for (int i = 0; i < m + 1; i++)
            {
                loopMap[i] = new Loop[n + 1];  // все ячейки будут null
            }
        }

        public LoopMap(int m, int n)
        {
            LoopMapId = new Guid();
            this.m = m;
            this.n = n;
            loopMap = new Loop[m][];
            for (int i = 0; i < loopMap.Length; ++i)
            {
                loopMap[i] = new Loop[n];
                for (int j = 0; j < loopMap[i].Length; ++j)
                {
                    loopMap[i][j] = new Loop(LoopType.loop, LoopSide.front); ;
                }
            }
        }

        public LoopMap(Guid id,Loop[][] loopMap)
        {
            LoopMapId = id;
            m = loopMap.Length;
            n = loopMap[0].Length;
            this.loopMap=loopMap;
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

        public void ColorLoopMap(string color)
        {
            if(loopMap == null) throw new ArgumentNullException("Схема не инициализирована");
            for (int i = 0; i < loopMap.Length; ++i)
            {
                for(int j = 0; j < loopMap[i].Length; ++j)
                {
                    if(loopMap[i][j]!=null&&loopMap[i][j].GetType()!=LoopType.none)
                    loopMap[i][j].Color = color;
                }
            }
        }
        public void AddLoop(int m, int n, LoopType type, LoopSide side)
        {
            loopMap[m][n] = new Loop(type,side);
        }

        public void ChangeColor(int m, int n, string color)
        {
            if (loopMap[m][n] != null && loopMap[m][n].GetType() != LoopType.none)
                loopMap[m][n].Color = color;
        }

        public void ChangeType(int m, int n, LoopType type)
        {
            loopMap[m][n].type = type;
        }

        void DraftToLoopMap(Draft draft)  //конвертация выкройки в матрицу петель
        {
           // Point nullPoint=draft.StartPoint;  //точка (0,0) в начале чертежа (середина низа)
           Point nullPoint= draft.GetUpperPoint(); // точка (0,0) левый верхний угол
            this.nullPoint = nullPoint;
            nullM= (int)((nullPoint.Y - draft.StartPoint.Y) / loopHeight) + AddingM / 2;
            nullN= (int)((draft.StartPoint.X - nullPoint.X) / loopWidth) + AddingN / 2;
            //   nullPoint.X += AddingN / 2;  //смещаем точку для дальнейшего расширения
            //  nullPoint.Y += AddingM / 2;

            // AddLoop(0, 0, LoopType.loop, LoopSide.front);
            int count = 1;
            foreach(var point in draft.draft)  //переносим контур выкройки в матрицу относительно нуля
              {
                if (point.Equals(draft.StartPoint) || point.Equals(draft.EndPoint)) //пропускаем начальную и конечную точки, так как они в центре
                    continue;
                int m=(int)((nullPoint.Y-point.Y)/loopHeight)+AddingM/2;
                int n=(int)((point.X-nullPoint.X)/loopWidth)+AddingN/2;
                LoopSide side;
                if (count % 2 != 0)
                    side = LoopSide.front;
                else 
                    side = LoopSide.back;
                AddLoop(m, n, LoopType.loop, side);
                ++count;
              }
            for(int i = loopMap.Length-1; i >= 0; i--)  //заполняем контур снизу вверх 
            {
                bool isOpen = false;  //маркер нахождения внутри контура
                LoopType type=LoopType.none;
                for( int j = loopMap[0].Length - 1; j >=0; j--)  //справа налево
                {
                    LoopSide side;
                    if (i % 2 != 0)  //нечетные ряды лицевые
                        side = LoopSide.front;
                    else
                        side = LoopSide.back;  //четные изнаночные
                    if (loopMap[i][j]!=null&&loopMap[i][j].GetType() == LoopType.loop)  //встречаем контур
                    {
                        if (isOpen)  //контур закрылся, заканчиваем заполнение
                        {
                            isOpen = false;
                            type = LoopType.none;
                        }
                        else  //дошли до контура, начинаем заполнение
                        {
                            isOpen= true;
                            type=LoopType.loop;
                        }
                    }
                    if(isOpen)
                    {
                        AddLoop(i, j, type, side);  //добавляем петлю нужного типа и вида
                    }
                    
                }
            }

            for (int i = 1; i < loopMap.Length-1; ++i)  //проставляем убавки и прибавки сверху вниз
            {
                for(int j=0; j < loopMap[i].Length; ++j)  //слева направо
                {
                    if (loopMap[i][j]!=null)
                    {
                        if ((loopMap[i][j].GetType() == LoopType.loop
                        && loopMap[i + 1][j] == null)||
                        (loopMap[i][j].GetType() == LoopType.loop  
                        && loopMap[i+1][j].GetType() == LoopType.none))  //убавка
                        {
                            loopMap[i][j].type = LoopType.decrease;
                        }
                        if ((loopMap[i][j].GetType() == LoopType.loop
                            && loopMap[i - 1][j]== null)||
                            (loopMap[i][j].GetType() == LoopType.loop
                            && loopMap[i - 1][j].GetType() == LoopType.none))  //прибавкуа
                        {
                            loopMap[i][j].type = LoopType.increase;
                        }
                    }
                }
            }

        }


        public void RebuildLoopMap(double leftX, double rightX, double topY, double lowY, List<Point> newPoints)
        {
            int lowM = (int)((nullPoint.Y - lowY) / loopHeight) + AddingM / 2; ;
            int topM = (int)((nullPoint.Y - topY) / loopHeight) + AddingM / 2; ;
            int rightN= (int)((rightX - nullPoint.X) / loopWidth) + AddingN / 2;
            int leftN= (int)((leftX - nullPoint.X) / loopWidth) + AddingN / 2;

            for (int i = lowM; i >= topM; --i)  //обнуляем старые петли
            {
                for (int j = leftN; j <=rightN; ++j)
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
                for(int h= rightN + 1; h >= leftN - 1; h--)
                {
                    if (loopMap[i][h] != null && loopMap[i][h].GetType() == LoopType.loop)
                        ++loopCount;
                }
                bool isOpen = false;  //маркер нахождения внутри контура
                LoopType type = LoopType.none;
                for (int j = rightN+1; j >= leftN-1; j--)  //справа налево
                {
                    LoopSide side;
                    if (i % 2 != 0)  //нечетные ряды лицевые
                        side = LoopSide.front;
                    else
                        side = LoopSide.back;  //четные изнаночные
                    if (loopMap[i][j] != null && loopMap[i][j].GetType() == LoopType.loop&&loopCount>1&&loopCount<=2)  //встречаем контур
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
