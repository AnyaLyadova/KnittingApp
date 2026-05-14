using KnittingApp.Parts;

namespace KnittingApp
{
    public class Form
    {
        public Guid formId {  get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public List<string> Parts {  get; set; }
        public Form(string name, Guid userId, List<string> Parts) {
            formId = Guid.NewGuid();
            Parts = new List<string>();
            Name = name;
            UserId = userId;
            this.Parts = Parts;
        }
    }
}
