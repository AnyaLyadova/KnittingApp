using static KnittingApp.SharedConstants;
namespace KnittingApp.Parts
{
    public class Armhole : Part  //выемка под рукав
    {
        /* public Armhole( double width, double height, int loopWidth, int loopHeight)
             : base(ArmholeName, width, height, loopWidth, loopHeight)
         {
         }*/

        double width;
        double height;

        public Armhole()
            : base(ArmholeName, PartPriority.Armhole)
        {
            partMeasures = ArmholeMeasures;
        }

        public override void InitializePart(Dictionary<string, double> measures, 
            double loopWidth, double loopHeight, double loopInWidht, double loopInHeight)
        {
            base.InitializePart(measures, loopWidth, loopHeight, loopInWidht, loopInHeight);

            width = partMeasures["armholeWidth"];
            height = partMeasures["armholeHeight"];
            Width = width;
        }

        public override Draft GetDraft(Point startPoint)
        {
            Draft armholeDraft = new Draft(startPoint);
            int armholeLoopHeight=(int)(height*loopInHeight);
            int armholeLoopWidht=(int)(width*loopInWidth);
            int part = armholeLoopWidht / 3;
            int firstPart = part;
            Point currentPoint=startPoint;
            if(armholeLoopWidht % 3 != 0)
            {
                firstPart = part + 1;
            }
            if (firstPart % 2 != 0)
            {
                Point p = new Point(startPoint.X + loopWidth * (firstPart/2) + 1, startPoint.Y + loopHeight, name); //убавка
                p.AddConnection(startPoint);
                armholeDraft.AddPoint(p);
                currentPoint = p;
            }
            else
            {
                Point p = new Point(startPoint.X + loopWidth * (firstPart / 2), startPoint.Y + loopHeight, name);
                p.AddConnection(startPoint);
                armholeDraft.AddPoint(p);
                currentPoint = p;
            }

            Point p1 = new Point(currentPoint.X + loopWidth * (firstPart / 2), currentPoint.Y + loopHeight, name);  //вторая убавка
            p1.AddConnection(startPoint);
            armholeDraft.AddPoint(p1);
            currentPoint = p1;

            int count = 2;
            for(int i = 1; i <= part; ++i)  //закрываем петли в каждом втором ряду
            {
                Point p=new Point(currentPoint.X+loopWidth, currentPoint.Y+loopHeight, name); //убавка
                p.AddConnection(currentPoint);
                armholeDraft.AddPoint(p);
                currentPoint = p;
                Point upper=new Point(currentPoint.X, currentPoint.Y+loopHeight, name); //подъем вверх
                upper.AddConnection(currentPoint);
                armholeDraft.AddPoint(upper);
                currentPoint = upper;
                ++count;
            }
            for(int i = 1; i <= part; ++i) //закрываем петли в каждом третьем ряду
            {
                Point p = new Point(currentPoint.X + loopWidth, currentPoint.Y + loopHeight, name); //убавка
                p.AddConnection(currentPoint);
                armholeDraft.AddPoint(p);
                currentPoint = p;
                ++count;

                    Point upper = new Point(currentPoint.X, currentPoint.Y + loopHeight*3, name); //3 раза подъем вверх
                    upper.AddConnection(currentPoint);
                    armholeDraft.AddPoint(upper);
                    currentPoint = upper;
                    count+=3;
            }
            armholeLoopHeight -= count;
            Point endPoint = new Point(currentPoint.X, currentPoint.Y + loopHeight * armholeLoopHeight, name);
            armholeDraft.AddPoint(endPoint);
            armholeDraft.EndPoint = endPoint;
            return armholeDraft;
        }
    }
}
