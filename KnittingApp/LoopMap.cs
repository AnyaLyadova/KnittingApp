using Microsoft.Extensions.Primitives;
using System.Drawing;
using static KnittingApp.SharedConstants;
namespace KnittingApp
{
    public class LoopMap
    {
        public Loop[][] loopMap {  get; }
        int m;
        int n;
        double loopWidth;
        double loopHeight;
        public LoopMap(Draft draft, double loopWidth, double loopHeight) {
            Point upper = draft.GetUpperPoint();
            Point bottom = draft.GetBottomPoint();
            int m = (int)(Math.Abs(upper.Y - bottom.Y)/loopHeight);
            int n = (int)(Math.Abs(upper.X - bottom.X)/loopWidth);
            this.m = m+1;
            this.n = n+1;
            this.loopWidth = loopWidth;
            this.loopHeight = loopHeight;
            loopMap = new Loop[m+1][];
            for (int i = 0; i < m+1; i++)
            {
                loopMap[i] = new Loop[n+1];  // все ячейки будут null
            }
            DraftToLoopMap(draft);
        }

        public LoopMap(int m, int n)
        {
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

        public bool IsFit(int x, int y, LoopMap pattern)
        {
            return x + pattern.loopMap.GetLength(0) <= loopMap.GetLength(0) //влезает по строкам
                 && y + pattern.loopMap.GetLength(1) <= loopMap.GetLength(1); // влезает по столбцам
        }

        public void AddPattern(int x, int y, LoopMap pattern)
        {
            if (pattern == null) throw new ArgumentNullException();
            if (!IsFit(x, y, pattern))
                throw new ArgumentException("Узор не помещается на полотно");
            for(int i = x; i<=pattern.loopMap.Length; ++i)
            {
                for(int j=y; j <= pattern.loopMap[i].Length; ++j)
                {
                    loopMap[i][j]=pattern.loopMap[i][j];
                }
            }
        }


        public void ColorLoopMap(string color)
        {
            if(loopMap == null) throw new ArgumentNullException("Схема не инициализирована");
            for (int i = 0; i < loopMap.Length; ++i)
            {
                for(int j = 0; j < loopMap[i].Length; ++j)
                {
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
           // AddLoop(0, 0, LoopType.loop, LoopSide.front);
            int count = 1;
            foreach(var point in draft.draft)  //переносим контур выкройки в матрицу относительно нуля
              {
                if (point == draft.StartPoint || point == draft.EndPoint) //пропускаем начальную и конечную точки, так как они в центре
                    continue;
                  int m=(int)((nullPoint.Y-point.Y)/loopHeight);
                  int n=(int)((point.X-nullPoint.X)/loopWidth);
                LoopSide side;
                if (count % 2 != 0)
                    side = LoopSide.front;
                else 
                    side = LoopSide.back;
                  AddLoop(m, n, LoopType.loop, side); 
              }
            for(int i = loopMap.Length-1; i >= 0; i++)  //заполняем контур снизу вверх 
            {
                bool isOpen = false;  //маркер нахождения внутри контура
                LoopType type=LoopType.none;
                for( int j = loopMap[0].Length - 1; j <=0; j--)  //справа налево
                {
                    LoopSide side;
                    if (i % 2 != 0)  //нечетные ряды лицевые
                        side = LoopSide.front;
                    else
                        side = LoopSide.back;  //четные изнаночные
                    if (loopMap[i][j].GetType() == LoopType.loop)  //встречаем контур
                    {
                        if (isOpen)  //контур закрылся, заканчиваем заполнение
                        {
                            isOpen = false;
                        }
                        else  //дошли до контура, начинаем заполнение
                        {
                            isOpen= true;
                        }
                    }
                    else
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




    }
}
