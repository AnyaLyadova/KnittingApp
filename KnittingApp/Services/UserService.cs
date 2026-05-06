using KnittingApp.Repository;
using KnittingApp.Extensions;

namespace KnittingApp.Services
{
    public class UserService:IUserService
    {
        public readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task<User> CreateUser(string username, string password)
        {
            Guid id = Guid.NewGuid();
            var user = new User(id, username, password).ToModel();
            var model = await userRepository.CreateUser(user);
            return model.ToObject();
        }

        public async Task<User> GetUserByPassword(string username, string password)
        {
            //бд
            var userModel=await userRepository.GetUserByPassword(username, password);
            return userModel.ToObject();
        }

        public async Task<User> GetUserById(Guid id)
        {
            var userModel = await userRepository.GetUserById(id);
            return userModel.ToObject();
        }
    }
}
