namespace KnittingApp.Services
{
    public interface IUserService
    {
        public Task<User> CreateUser(string username, string password);
        public Task<User> GetUserByPassword(string username, string password);
        public Task<User> GetUserById(Guid id);


    }
}
