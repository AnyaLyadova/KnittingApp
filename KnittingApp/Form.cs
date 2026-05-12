using KnittingApp.Parts;

namespace KnittingApp
{
    public class Form
    {
        public Guid formId;
        public List<Part> Parts {  get; set; }
        public Form() {
            formId = Guid.NewGuid();
            Parts = new List<Part>();
        }
        public void AddPart(Part part)
        {
            Parts.Add(part);
        }
    }
}
