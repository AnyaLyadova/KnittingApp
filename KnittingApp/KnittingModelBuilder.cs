using KnittingApp.Parts;

namespace KnittingApp
{
    public class KnittingModelBuilder
    {
        List<Part> parts = new List<Part>{new Armhole(), new Body(), new Neck(), new Quad(),
        new Sleeve(), new SleeveRoll(), new Shoulder(), new VNeck()}; 
        public Model CreateModel(string name,List<string> stringParts, Guid userId/*,double loopWidth, double loopHeight*//*, Dictionary<string, double> measures*/)
        {
            Model model=new(name, userId/*, loopWidth, loopHeight*//*, measures*/);
            model.AddPart(new Body());
            model.AddPart(new Shoulder());
            model.AddPart(new Sleeve());
            model.AddPart(new SleeveRoll());
            foreach(var str in stringParts)
            {
                if (str == "none")
                    continue;
                Part part = parts.FirstOrDefault(p => p.name == str);
                if (part == null)
                    throw new InvalidDataException("Требуемая часть отсутсвует в списке "+str);
                model.AddPart(part);
            }
            foreach(Part part in model.Parts)
            {
                model.AddMeasure(part.PartMeasures);
            }
            model.Parts.Sort((a, b) => a.GetPriority().CompareTo(b.GetPriority()));

            return model;
        }
    }
}
