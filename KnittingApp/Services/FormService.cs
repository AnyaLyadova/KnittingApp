using KnittingApp.Repository;

namespace KnittingApp.Services
{
    public class FormService:IFormService
    {
        private readonly IFormRepository formRepository;
        public FormService(IFormRepository formRepository)
        {
            this.formRepository = formRepository;
        }
        public async Task<Form> CreateForm(string name, Guid userId, List<string> Parts)
        {
            var form=new Form(name, userId, Parts);
            await formRepository.CreateForm(form);
            return form;
        }
        public async Task<Form> GetForm(Guid id)
        {
            var form=await formRepository.GetForm(id);
            return form;
        }
        public async Task<List<Form>> GetFormsByUser(Guid userId)
        {
            var form=await formRepository.GetFormsByUser(userId);
            return form;
        }
    }
}
