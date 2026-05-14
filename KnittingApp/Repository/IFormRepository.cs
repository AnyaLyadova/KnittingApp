namespace KnittingApp.Repository
{
    public interface IFormRepository
    {
        public Task<Form> CreateForm(Form form);
        public Task<Form> GetForm(Guid id);
        public Task<List<Form>> GetFormsByUser(Guid userId);
    }
}
