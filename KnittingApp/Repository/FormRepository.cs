using Microsoft.EntityFrameworkCore;

namespace KnittingApp.Repository
{
    public class FormRepository:IFormRepository
    {
        private readonly AppDbContext _context;
        public FormRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Form> CreateForm(Form form)
        {
            if (form == null)
            {
                throw new ArgumentNullException("Переданная форма была  null");
            }
            _context.Forms.Add(form);
            await _context.SaveChangesAsync();
            return form;
        }
        public async Task<Form> GetForm(Guid id)
        {
            var form=await _context.Forms.Where(f=>f.formId==id).FirstOrDefaultAsync();
            if (form==null)
                throw new ArgumentException($"Формы с id {id} не найдено");
            return form;
        }
        public async Task<List<Form>> GetFormsByUser(Guid userId)
        {
            var forms=await _context.Forms.Where(f=>f.UserId==userId).ToListAsync();
            return forms;
        }
    }
}
