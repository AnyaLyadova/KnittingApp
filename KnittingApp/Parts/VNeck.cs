using static KnittingApp.SharedConstants;
namespace KnittingApp.Parts
{
    public class VNeck:Part
    {
        public VNeck()
           : base(VNeckName, PartPriority.Neck)
        {
            partMeasures = NeckMeasures;
        }

        int ratio;
        double width;
        double height;


        public override void InitializePart(Dictionary<string, double> measures,
            double loopWidth, double loopHeight, double loopInWidth, double loopInHeight)
        {
            base.InitializePart(measures, loopWidth, loopHeight, loopInWidth, loopInHeight);

            {
                if (isFront)
                    ratio = 2;
                else
                    ratio = 4;
            }
            width = partMeasures["neckWidth"];
            height = partMeasures["neckHeight"];
            Width = width;
        }

        public override Draft GetDraft(Point startPoint)
        {
            Draft neckDraft = new Draft(startPoint);
            if (isFront)
            {
                int neckLoopWidth = (int)(width * loopInWidth);  //количество петель горловины
                int neckLoopHeight= (int)(height * loopInHeight);
                int increase = (int)(neckLoopWidth / 2);  //делим пополам
                int line = (int)(neckLoopHeight / increase);
                Point currentPoint = startPoint;
                currentPoint = new Point(startPoint.X+loopInWidth, startPoint.Y, name);
                neckDraft.AddPoint(currentPoint);
                line = (line == 0) ? 1 : line;
                for (int i = 1; i <= neckLoopHeight; ++i)
                {
                    Point p = new Point(currentPoint.X, currentPoint.Y - loopHeight, name);
                    if (i % line == 0)
                    {
                        p.X += loopWidth;  //делаем убавку
                    }
                    //   p.AddConnection(currentPoint);
                   neckDraft.AddPoint(p);
                    currentPoint = p;
                }
                neckDraft.EndPoint = currentPoint;
            }
            else
            {

                int neckLoopWidth = (int)(width * loopInWidth);  //количество петель горловины
                int lowWidth = (neckLoopWidth * 4) / (6 * ratio);  //ширина центральной части горловины
                int increase = (int)((neckLoopWidth - lowWidth) / 2); //количество убавок на каждую сторону
                int neckLoopHeight = (int)(height * loopInHeight);  //количество рядов
                if (neckLoopHeight % 2 != 0)
                    ++neckLoopHeight;
                Point currentPoint = startPoint;
                Point adding = new Point(currentPoint.X + loopInWidth, currentPoint.Y, name);
                neckDraft.AddPoint(adding);
                currentPoint = adding;
                for (int i = 1; i < neckLoopHeight; ++i)  //делаем прибавки в цилке
                {
                    Point p = new Point(currentPoint.X, currentPoint.Y - loopHeight, name);
                    if (i % 2 != 0)
                    {
                        p.X += loopWidth * (increase / 2);  //прибавки через ряд
                        increase -= increase / 2;  //уменьшаем общее количество убавок
                    }
                    neckDraft.AddPoint(p);
                    currentPoint = p;
                }
                Point endPoint = new Point(currentPoint.X + lowWidth * loopWidth / 2, currentPoint.Y, name);
                neckDraft.AddPoint(endPoint);  //добавляем половину центральной части
                neckDraft.EndPoint = endPoint;
                neckDraft.EndPoint.visible = true;
                neckDraft.StartPoint.visible = true;
            }
            return neckDraft;
        }

        public void ChangeWidth(double width)
        {
            this.width += width;
        }
    }
}
