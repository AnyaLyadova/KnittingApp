using static KnittingApp.SharedConstants;
namespace KnittingApp.Parts
{
    public class Body :Part
    {
        /* public Body( double topWidth, double bottomWidth, double height, int loopWidth, int loopHeight)
             : base(BodyName, topWidth, height, loopWidth, loopHeight)
         {
             this.bottomWidth = bottomWidth;
             this.topWidth = topWidth;
         }*/
        public Body()
            : base(BodyName, PartPriority.Body)
        {
            this.partMeasures = BodyMeasures;
        }
       // double width;
        double height;
        double topWidth;
        double bottomWidth;

        public double TopWidth {get {return this.topWidth;}}
        public double BottomWidth {get {return this.bottomWidth;}}

        public override void InitializePart(Dictionary<string, double> measures, 
            double loopWidth, double loopHeight, double loopInWidth, double loopInHeight)
        {
            base.InitializePart(measures, loopWidth, loopHeight, loopInWidth, loopInHeight);
            height = partMeasures["bodyHeight"];
            topWidth = partMeasures["topWidth"];
            bottomWidth = partMeasures["bottomWidth"];
        }
        public override Draft GetDraft(Point startPoint)
        {
            Draft bodyDraft = new Draft(startPoint);
            int bodyTopLoopWidht = (int)(topWidth * loopInWidth);
            int bodyBottopLoopWidht = (int)(bottomWidth * loopInWidth);
            int bodyLoopHeight = (int)(height * loopInHeight);

           
            Point currentPoint = new Point(startPoint.X - (bodyBottopLoopWidht*loopWidth / 2), startPoint.Y, name);
            currentPoint.AddConnection(startPoint);
            bodyDraft.AddPoint(currentPoint);
            if (bottomWidth > topWidth)  //если нижняя часть шире верхней
            {
                int loops=bodyBottopLoopWidht/2 - bodyTopLoopWidht/2;
                int line = bodyLoopHeight / loops;
                for(int i=1; i <= bodyLoopHeight; ++i)
                {
                    Point p=new Point(currentPoint.X, currentPoint.Y+loopHeight, name);
                    if(i%loops == 0)
                    {
                        p.X += loopWidth;  //делаем убавку
                    }
                    p.AddConnection(currentPoint);
                    bodyDraft.AddPoint(p);
                    currentPoint = p;
                }
                bodyDraft.EndPoint = currentPoint;
            }
            else if(bottomWidth<topWidth)  //если верхняя часть шире нижней
            {
                int loops = bodyTopLoopWidht-bodyBottopLoopWidht;
                int line = bodyLoopHeight / loops;
                for (int i = 1; i <= bodyLoopHeight; ++i)
                {
                    Point p = new Point(currentPoint.X, currentPoint.Y + loopHeight, name);
                    if (i % loops == 0)
                    {
                        p.X -= loopWidth;  //делаем прибавку
                    }
                    p.AddConnection(currentPoint);
                    bodyDraft.AddPoint(p);
                    currentPoint = p;
                }
                bodyDraft.EndPoint=currentPoint;
            }
            else  //если верхняя и нижняя часть равны
            {
                Point p=new Point(currentPoint.X,currentPoint.Y+bodyLoopHeight, name);  //добавляем высоту
                p.AddConnection(currentPoint);
                bodyDraft.AddPoint(p);
                bodyDraft.EndPoint = p;

            }


                return bodyDraft;
        }
    }
}
