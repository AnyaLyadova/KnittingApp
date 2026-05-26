using static KnittingApp.SharedConstants;
namespace KnittingApp.Parts
{
    public class Neck : Part  //круглая горловина
    {
        int ratio;
        double width;
        double height;
        /* public Neck(string name, double width, double height, int loopWidth, int loopHeight, bool isFront) 
             : base(ONeckName, width, height, loopWidth, loopHeight)
         {
             if (isFront)
                 ratio = 2;
             else
                 ratio = 4;
         }*/

        public Neck()
           : base(ONeckName, PartPriority.Neck)
        {
            partMeasures = NeckMeasures;
        }

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
            Draft neckDraft=new Draft(startPoint);
            int neckLoopWidth=(int)(width*loopInWidth);  //количество петель горловины
            int lowWidth = (neckLoopWidth*4) / (6*ratio);  //ширина центральной части горловины
            int increase=(int)((neckLoopWidth - lowWidth)/2); //количество убавок на каждую сторону
            int neckLoopHeight = (int)(height * loopInHeight);  //количество рядов
            if (neckLoopHeight % 2 != 0)
                ++neckLoopHeight;
            Point currentPoint = startPoint;
            Point adding = new Point(currentPoint.X+loopInWidth, currentPoint.Y, name);
            neckDraft.AddPoint(adding);
            currentPoint = adding;
            for (int i = 1; i < neckLoopHeight; ++i)  //делаем прибавки в цилке
            {
                Point p = new Point(currentPoint.X, currentPoint.Y-loopHeight, name);
                if (i % 2 != 0)
                {
                    p.X += loopWidth*(increase/2);  //прибавки через ряд
                    increase -= increase / 2;  //уменьшаем общее количество убавок
                }
                neckDraft.AddPoint(p);
                currentPoint = p;
            }
            Point endPoint= new Point(currentPoint.X + lowWidth*loopWidth / 2, currentPoint.Y, name);
            neckDraft.AddPoint(endPoint);  //добавляем половину центральной части
            neckDraft.EndPoint = endPoint;
            neckDraft.EndPoint.visible = true;
            neckDraft.StartPoint.visible = true;
            return neckDraft;
        }

        public void ChangeWidth(double width)
        {
            this.width += width;
        }
    }
}
