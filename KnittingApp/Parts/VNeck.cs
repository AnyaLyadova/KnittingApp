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
    }
}
