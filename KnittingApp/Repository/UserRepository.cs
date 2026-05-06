using KnittingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace KnittingApp.Repository
{
    public class UserRepository:IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel> GetUserById(Guid id)
        {
            var user=await _context.Users.Where(u=>u.UserId==id).FirstOrDefaultAsync();
            if (user==null)
                throw new ArgumentException($"Пользователя с id {id} не существует");
            return user;
        }

        public async Task<UserModel> GetUserByPassword(string username, string password)
        {
            var user=await _context.Users.Where(u=> u.Username==username).FirstOrDefaultAsync();
            if(user==null)
                throw new ArgumentException($"Пользователя с логином {username} не существует");
            if (user.Password != password)
                throw new ArgumentException("Пароль введен неверно");
            return user;
        }

        public async Task<UserModel> CreateUser(UserModel user)
        {
            if (user == null)
                throw new ArgumentException("Переданный пользватель был null");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

    }
}
