using static System.Net.Mime.MediaTypeNames;
using static KnittingApp.SharedConstants;
namespace KnittingApp
{
    public class Draft
    {
        public LinkedList<Point> draft { get; private set; }
        Point startPoint;
        Point endPoint;

        public Draft(Point startPoint)
        {
            this.startPoint = startPoint;
            draft = new LinkedList<Point>();
            draft.AddLast(startPoint);
        }

        public Draft(Draft draft)
        {
            this.draft= draft.draft;
            startPoint=draft.startPoint;
            endPoint=draft.endPoint;
        }


        public Draft(List<Draft> drafts)  //склеиваем чертеж из раных частей
        {
            this.draft = drafts[0].draft;
            startPoint = drafts[0].startPoint;
            endPoint = drafts[0].endPoint;
            //Draft fullDraft = new Draft(drafts[0]);
            for (int i = 1; i < drafts.Count; ++i)
            {
                if (this.endPoint == drafts[i].startPoint)
                {
                    /* for (int j = 1; j < drafts[i].draft.Count; ++j)
                     {
                         this.AddPoint(drafts[i].draft[j]);
                     }
                     this.endPoint = drafts[i].endPoint;*/

                    for (var loop = drafts[i].draft.First.Next; loop!=null; loop=loop.Next)
                    {
                        this.AddPoint(loop.Value);
                    }
                    this.endPoint = drafts[i].endPoint;
                }
                else
                {
                    throw new InvalidDataException("Невозможно объединить чертежи: отсутствует точка соприкосновения");
                }
            }

            //return fullDraft;
        }

        public Point StartPoint { get { return startPoint; } }
        public Point EndPoint { get { return endPoint; } set { endPoint = value; } }



        public void  MovePoint(Point p, int new_x, int new_y)
        {
           
        }

        public void AddPoint(Point p)  //добавляет точку чертежа
        {
            /*foreach (Point point in p.GetConnections()) { 
                point.AddConnection(p);          
            }*/

            draft.AddLast(p);
           
        }

        public bool DeletePoint(Point p)  //удаляет точку чертежа
        { 
            throw new Exception();
        }

        public bool SaveDraft()
        {
            throw new Exception();
        }

        public void PrintAsPNG()
        {

        }

        public void PrintAsPDF()
        {

        }

        public bool CheckClosed() //проверка на замкнутость контура
        {
            return startPoint==endPoint;
        }

        public bool DeleteLine()  //убирает лишнюю линию, когда контур замыкается
        {
            throw new Exception();
        }

       /* public Draft CreateDraft(List<Draft> drafts)  //объединение нескольких частей в единую выкройку
        {
            foreach (Draft d in drafts) 
            { 
                draft.Concat(d.draft).Distinct().ToList();
            }
            return this;
        }*/

        public Point GetUpperPoint()  //левый верхний угол
        {
            double x=draft.Min(p=>p.X);
            double y=draft.Max(p=>p.Y);
            return new Point(x, y, "none");
        }

        public Point GetBottomPoint()  //правый нижний угол
        {
            double x = draft.Max(p => p.X);
            double y = draft.Min(p => p.Y);
            return new Point(x, y, "none");
        }

        
        public Draft Mirror()
        {
            Draft mirroredDraft = new Draft(endPoint);
            var connectionPoint = endPoint;  //для добавления связи в точку

            // Проходим по всем точкам оригинала (кроме startPoint)
            /*for (int i = draft.Count-1; i >=1; i--)
            {
                var originalPoint = draft[i];

                var mirroredPoint = MirrorPoint(originalPoint);  //отражаем точку
                mirroredPoint.AddConnection(connectionPoint); //добавляем связь в точку
                connectionPoint = mirroredPoint;
                mirroredDraft.draft.AddLast(mirroredPoint);
            }*/

            for (var loop=draft.Last; loop!=null; loop=loop.Previous)
            {
                var originalPoint = loop;

                var mirroredPoint = MirrorPoint(originalPoint.Value);  //отражаем точку
              //  mirroredPoint.AddConnection(connectionPoint); //добавляем связь в точку
                connectionPoint = mirroredPoint;
                mirroredDraft.draft.AddLast(mirroredPoint);
            }

            if (startPoint != null)
            {
                /* var mirroredEndPoint = MirrorPoint(startPoint); //отражаем конечную точку
                  mirroredDraft.draft.AddLast(mirroredEndPoint);
                  mirroredDraft.EndPoint = mirroredEndPoint;*/
                mirroredDraft.draft.AddLast(startPoint);
                mirroredDraft.EndPoint=startPoint;
            }

            foreach (Point point in mirroredDraft.draft) 
            {
                draft.AddLast(point);
            }

            return mirroredDraft;
        }


        private Point MirrorPoint(Point source)
        {
            double centerX = endPoint.X;  //отражаем относительно стартовой точки (середина низа)
            double distanceFromCenter = source.X - centerX;
            double mirroredX = centerX - distanceFromCenter;
            var mirroredPoint = new Point(mirroredX, source.Y, source.Part);
            return mirroredPoint;
        }

        

    }
}
