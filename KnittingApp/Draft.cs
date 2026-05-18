
using System.Collections.Generic;
using System.Xml.Linq;
using static KnittingApp.SharedConstants;
using static System.Net.Mime.MediaTypeNames;
namespace KnittingApp
{
    public class Draft
    {
        public Guid DraftId { get; set; }
        public LinkedList<Point> draft { get; private set; }
        Point startPoint;
        Point endPoint;

        public Point StartPoint { get { return startPoint; } }
        public Point EndPoint { get { return endPoint; } set { endPoint = value; } }

        public Draft(Point startPoint)
        {
           
            this.startPoint = startPoint;
            draft = new LinkedList<Point>();
            draft.AddLast(startPoint);
        }

        public Draft(Guid id, LinkedList<Point> points)
        {
            DraftId = id;
            draft = points;
            startPoint = points.First();
        }

        public Draft(Draft draft)
        {
            this.draft= draft.draft;
            startPoint=draft.startPoint;
            endPoint=draft.endPoint;
        }


        public Draft(List<Draft> drafts)  //склеиваем чертеж из раных частей
        {
            DraftId=Guid.NewGuid();
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
            mirroredPoint.visible=source.visible;
            return mirroredPoint;
        }


        public void MovePoint(Point movingPoint, double newX, double newY, Point leftPoint, Point rightPoint, double loopWidth, 
            double loopHeight, double loopInWidth, double loopInHeight, out List<Point> newPoints)
        {
            var leftNode = draft.Find(leftPoint);
            var moveNode = draft.Find(movingPoint);
            LinkedListNode<Point> currentNode;
            LinkedListNode<Point> searchNode;
            newPoints=new List<Point> ();
            if (IsNodeBefore(leftNode, moveNode))
            {
                currentNode = draft.Find(leftPoint).Next;      //удаляем точки между движимой и опорными
                searchNode= draft.Find(movingPoint);
            }
            else
            {
                currentNode = draft.Find(movingPoint).Next; 
                searchNode= draft.Find(leftPoint);
            }
                //   var currentNode=draft.Find(leftPoint).Next;
                while (currentNode != searchNode)
                {
                    var next = currentNode.Next;
                    draft.Remove(currentNode);
                    currentNode = next;
                }
            var rightNode= draft.Find(rightPoint);
            if (IsNodeBefore(moveNode, rightNode))
            {
                currentNode = draft.Find(movingPoint).Next;
                searchNode = draft.Find(rightPoint);
            }
            else
            {
                currentNode = draft.Find(rightPoint).Next;
                searchNode = draft.Find(movingPoint);
            }
            while (currentNode != searchNode)
                {
                    var next = currentNode.Next;
                    draft.Remove(currentNode);
                    currentNode = next;
                }
            /*draft.Find(movingPoint).Value.X = newX;
            movingPoint.X = newX;
            draft.Find(movingPoint).Value.Y = newY;
            movingPoint.Y = newY;*/
            moveNode.Value.X = newX;
            moveNode.Value.Y = newY;

            movingPoint.X = newX;
            movingPoint.Y = newY;

            double leftWidth= Math.Abs(leftPoint.X-movingPoint.X);
            double leftHeight= Math.Abs(leftPoint.Y-movingPoint.Y);
            bool heightFlag = true;  
            bool widhtFlag = true;
            if(leftWidth==0)
                widhtFlag = false;  //если движемся ровно горизонтально
            if(leftHeight==0)
                heightFlag = false;  //если движемся ровно вертикально
            int leftLoopWidht = (int)(leftWidth * loopInWidth);
            int leftLoopHeight=(int)(leftHeight * loopInHeight);
            //var currentPoint = leftPoint;
            var currentPoint= draft.Find(leftPoint);
            /* if (leftPoint.X > movingPoint.X)
                 currentPoint = movingPoint;*/
            leftLoopWidht=leftLoopWidht == 0 ? 1:leftLoopWidht;
            int line = (int)(leftLoopHeight / leftLoopWidht);
            if (line == 0)
                line = 1;
            for (int i = 1; i <= leftLoopHeight; ++i)   //добавляем новые точки от левой точки
            {
                Point p = new Point(currentPoint.Value.X, currentPoint.Value.Y,"user");
                if (leftPoint.Y > movingPoint.Y&&heightFlag)
                    p.Y -= loopHeight;
                else if(heightFlag)
                    p.Y += loopHeight;
                if (i % line == 0&&widhtFlag)
                {
                    if(leftPoint.X>movingPoint.X)
                        p.X -= loopWidth;  //делаем убавку
                    else
                        p.X += loopWidth;
                }
                draft.AddAfter(currentPoint, p);
                newPoints.Add(p);
                currentPoint = currentPoint.Next;
            }

            //currentPoint = movingPoint;
            currentPoint = draft.Find(movingPoint);
            /*if(movingPoint.X>rightPoint.X)
                currentPoint=rightPoint;*/
            heightFlag = true;
            widhtFlag = true;
            double rightWidth = Math.Abs(rightPoint.X - movingPoint.X);
            double rightHeight = Math.Abs(rightPoint.Y - movingPoint.Y);
            if (rightWidth == 0)
                widhtFlag = false;
            if(rightHeight == 0)
                heightFlag = false;
            int rightLoopWidht = (int)(rightWidth * loopInWidth);
            int rightLoopHeight = (int)(rightHeight * loopInHeight);
            rightLoopWidht = rightLoopWidht == 0 ? 1 : rightLoopWidht;
            line = (int)(rightLoopHeight / rightLoopWidht);
            if (line == 0)
                line = 1;
            for (int i = 1; i <= rightLoopHeight; ++i)      //добавляем точки до правой точки
            {
                Point p = new Point(currentPoint.Value.X, currentPoint.Value.Y, "user");
                if (rightPoint.Y > movingPoint.Y &&heightFlag)
                    p.Y += loopHeight;
                else if(heightFlag)
                    p.Y-=loopHeight;
                if (i % line == 0&&widhtFlag)
                {
                    if(rightPoint.X>movingPoint.X)
                        p.X += loopWidth;  //делаем убавку
                    else
                        p.X -= loopWidth;
                }
                //   p.AddConnection(currentPoint);
                draft.AddAfter(currentPoint, p);
                newPoints.Add(p);
                currentPoint = currentPoint.Next;
            }
    }

        bool IsNodeBefore(LinkedListNode<Point> a, LinkedListNode<Point> b)
        {
            var current = a;
            while (current != null)
            {
                if (current == b) return true;
                current = current.Next;
            }
            return false;
        }


    }
}
