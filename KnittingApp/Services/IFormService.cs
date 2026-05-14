namespace KnittingApp.Services
{
    public interface IFormService
    {
        public Task<Form> CreateForm(string name, Guid userId, List<string> Parts);
        public Task<Form> GetForm(Guid id);
        public Task<List<Form>> GetFormsByUser(Guid userId);
    }
}
