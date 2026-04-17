using static KnittingApp.SharedConstants;
namespace KnittingApp.Parts
{
    public class Sleeve :Part  //рукав
    {
        /* public Sleeve( double width, double height, int loopWidth, int loopHeight)
             : base( SleeveName, width, height, loopWidth, loopHeight)
         {
         }*/
        double width;
        double height;

        public Sleeve()
            : base(SleeveName, PartPriority.Sleeve)
        {
            partMeasures = SleeveMeasures;
        }

        public override void InitializePart(Dictionary<string, double> measures, 
            double loopWidth, double loopHeight, double loopInWidht, double loopInHeight)
        {
            base.InitializePart(measures, loopWidth, loopHeight, loopInWidht, loopInHeight);

            width = partMeasures["sleeveWidth"];
            height = partMeasures["sleeveHeight"];
            Width = width;
        }

        public override Draft GetDraft(Point startPoint)
        {
            Draft sleeveDraft=new Draft(startPoint);
            int sleeveLoopWidth=(int)(width*loopInWidth);
            int sleeveLoopHeight= (int)(height*loopInHeight);
            int turns = sleeveLoopHeight / 2;
            int loops = sleeveLoopWidth / turns; //сколько петель закрывать за раз
            Point currentPoint=startPoint;
            for(int i = 1; i <= sleeveLoopHeight; ++i)
            {
                Point p=new Point(currentPoint.X, currentPoint.Y+loopHeight, name);
                if (i % turns == 0)
                {
                    p.X += loopWidth * loops;
                }
                p.AddConnection(currentPoint);
                sleeveDraft.AddPoint(p);
                currentPoint=p;
            }
            sleeveDraft.EndPoint = currentPoint;

            return sleeveDraft;
        }
    }
}
