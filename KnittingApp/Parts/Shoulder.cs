using static KnittingApp.SharedConstants;
namespace KnittingApp.Parts
{
    public class Shoulder : Part  //плечо
     {
        double width;
        double height;
        public Shoulder()
        : base(ShoulderName, PartPriority.Shoulder)
         {
            partMeasures = ShoulderMeasures;
         }

        public override void InitializePart(Dictionary<string, double> measures,
            double loopWidth, double loopHeight, double loopInWidht, double loopInHeight)
        {
            base.InitializePart(measures, loopWidth, loopHeight, loopInWidht, loopInHeight);

            width = partMeasures["shoulderWidth"];
            height = partMeasures["shoulderHeight"];
            Width = width;
        }
        public override Draft GetDraft(Point startPoint)
         {
            Draft shoulderDraft = new Draft(startPoint);
            int shoulderLoopHeight = (int)(height * loopInHeight);  //высота плеча в петлях
            int shoulderLoopWidht = (int)(width * loopInWidth);  //длина плеча в петлях
            int rows = shoulderLoopHeight / 2; //закрываем петли через раз
            int loops = shoulderLoopWidht / rows; //количество убавок в каждом ряду
            Point currentPoint = startPoint;
            for(int i=1; i <= shoulderLoopHeight; ++i)
            {
                Point p = new Point(currentPoint.X, currentPoint.Y + loopHeight, name);
                if (i % 2 == 0)
                {
                    p.X += loopWidth*loops;  //делаем убавку
                }
           //     p.AddConnection(currentPoint);
                shoulderDraft.AddPoint(p);
                currentPoint = p;
            }
            shoulderDraft.EndPoint=currentPoint;

            shoulderDraft.StartPoint.visible = true;
            shoulderDraft.EndPoint.visible = true;
            return shoulderDraft;
        }
       }
}
