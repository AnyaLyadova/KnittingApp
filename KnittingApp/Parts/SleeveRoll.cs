using static KnittingApp.SharedConstants;
namespace KnittingApp.Parts
{
    public class SleeveRoll : Part  //окат рукава
    {
        /* public SleeveRoll(double width, double height, int loopWidth, int loopHeight)
             : base(SleeveRollName, width, height, loopWidth, loopHeight)
         {
         }*/
        double width;
        double height;

        public SleeveRoll()
            : base(SleeveRollName, PartPriority.SleeveRoll)
        {
            partMeasures = SleeveRollMeasures;
        }

        public override void InitializePart(Dictionary<string, double> measures,
            double loopWidth, double loopHeight, double loopInWidht, double loopInHeight)
        {
            base.InitializePart(measures, loopWidth, loopHeight, loopInWidht, loopInHeight);

            width = partMeasures["sleeveRollWidth"];
            height = partMeasures["sleeveRollHeight"];
            Width = width;
        }

        public override Draft GetDraft(Point startPoint)
        {
            Draft rollDraft=new Draft(startPoint);
            int rollLoopWidht=(int)(width*loopInWidth/2);
            int rollLoopHeight=(int)(height*loopInHeight);
            int part = rollLoopWidht / 3;
            int firstPart = part;
            Point currentPoint = startPoint;
            if (rollLoopWidht % 3 != 0)
                firstPart++;
            int add = 0;
            if(firstPart%2!=0)
                add++;
            int partFirstPart = firstPart / 2;
            for(int i = 1; i <= (partFirstPart + add) / 3; ++i)  //по 3 убавки через ряд
            {
                Point p=new Point(currentPoint.X+loopWidth*3, currentPoint.Y+loopHeight, name);
           //     p.AddConnection(currentPoint);
                rollDraft.AddPoint(p);
                currentPoint = p;
                Point upper=new Point(currentPoint.X, currentPoint.Y+loopHeight, name);
               // upper.AddConnection(currentPoint);
                rollDraft.AddPoint(upper);
                currentPoint = upper;
            }

            for (int i = 1; i <= partFirstPart / 2; ++i)  //по 2 убавки через ряд
            {
                Point p = new Point(currentPoint.X + loopWidth * 2, currentPoint.Y + loopHeight, name);
               // p.AddConnection(currentPoint);
                rollDraft.AddPoint(p);
                currentPoint = p;
                Point upper = new Point(currentPoint.X, currentPoint.Y + loopHeight, name);
                //upper.AddConnection(currentPoint);
                rollDraft.AddPoint(upper);
                currentPoint = upper;
            }

            add = 0;
            if (part % 3 != 0)
                add = 1;
            for(int i = 1; i <= (part + add) / 3; ++i)  //убавка в каждом 2 ряду
            {
                Point p = new Point(currentPoint.X + loopWidth, currentPoint.Y + loopHeight, name);
             //   p.AddConnection(currentPoint);
                rollDraft.AddPoint(p);
                currentPoint = p;
                Point upper = new Point(currentPoint.X, currentPoint.Y + loopHeight, name);
              //  upper.AddConnection(currentPoint);
                rollDraft.AddPoint(upper);
                currentPoint = upper;
            }

            for (int i = 1; i <= part / 3; ++i)  //убавка в каждом 4 ряду
            {
                Point p = new Point(currentPoint.X + loopWidth, currentPoint.Y + loopHeight, name);
               // p.AddConnection(currentPoint);
                rollDraft.AddPoint(p);
                currentPoint = p;
                Point upper = new Point(currentPoint.X, currentPoint.Y + loopHeight*3, name);
               // upper.AddConnection(currentPoint);
                rollDraft.AddPoint(upper);
                currentPoint = upper;
            }

            for (int i = 1; i <= part / 3; ++i)  //убавка в каждом 2 ряду
            {
                Point p = new Point(currentPoint.X + loopWidth, currentPoint.Y + loopHeight, name);
               // p.AddConnection(currentPoint);
                rollDraft.AddPoint(p);
                currentPoint = p;
                Point upper = new Point(currentPoint.X, currentPoint.Y + loopHeight, name);
               // upper.AddConnection(currentPoint);
                rollDraft.AddPoint(upper);
                currentPoint = upper;
            }

            for (int i = 1; i <= part / 3; ++i)  //по 3 убавки в каждом 2 ряду
            {
                Point p = new Point(currentPoint.X + loopWidth*3, currentPoint.Y + loopHeight, name);
              //  p.AddConnection(currentPoint);
                rollDraft.AddPoint(p);
                currentPoint = p;
                Point upper = new Point(currentPoint.X, currentPoint.Y + loopHeight, name);
               // upper.AddConnection(currentPoint);
                rollDraft.AddPoint(upper);
                currentPoint = upper;
            }
            rollDraft.EndPoint = currentPoint;
            rollDraft.EndPoint.visible = true;
            rollDraft.StartPoint.visible = true;
            return rollDraft;
        }
    }
}
