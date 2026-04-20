using static KnittingApp.SharedConstants;
namespace KnittingApp.Parts
{
    public class Quad :Part  //четырыхугольник - для тела
    {

        /* public Quad( double topWidth, double bottomWidth, double height, int loopWidth, int loopHeight)
            : base(QuadName, topWidth, height, loopWidth, loopHeight)
         {
             this.bottomWidth = bottomWidth;
             this.topWidth = topWidth;   
         }*/

        public Quad()
          : base(QuadName, PartPriority.Body)
        {
            partMeasures = QuadMeasures;
        }
        double height;
        double topWidth;
        double bottomWidth;

        public override void InitializePart(Dictionary<string, double> measures, 
            double loopWidth, double loopHeight, double loopInWidht, double loopInHeight)
        {
            base.InitializePart(measures, loopWidth, loopHeight, loopInWidht, loopInHeight);
            height = partMeasures["quadHeight"];
            topWidth = partMeasures["topWidth"];
            bottomWidth = partMeasures["bottomWidth"];
            Width = bottomWidth;
        }

        public override Draft GetDraft(Point startPoint)
        {
            Draft bodyDraft=new Draft(startPoint);
            int bodyTopLoopWidht=(int)(topWidth*loopInWidth);
            int bodyBottopLoopWidht=(int)(bottomWidth*loopInWidth);
            int bodyLoopHeight=(int)(height*loopInHeight);
            Point currentPoint = new Point(startPoint.X-(bodyBottopLoopWidht/2), startPoint.Y, name);
          //  currentPoint.AddConnection(currentPoint);
            bodyDraft.AddPoint(currentPoint);
            if (bottomWidth > topWidth)
            {

            }
            else
            {

            }


                return bodyDraft;
        }
    }
}
