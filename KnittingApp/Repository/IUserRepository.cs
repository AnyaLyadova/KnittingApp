using KnittingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace KnittingApp.Repository
{
    public interface IUserRepository
    {
        public Task<UserModel> GetUserById(Guid id);

        public  Task<UserModel> GetUserByPassword(string username, string password);

        public Task<UserModel> CreateUser(UserModel user);
    }
}
