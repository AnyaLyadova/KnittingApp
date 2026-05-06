using KnittingApp.Repository;
using System.Threading.Tasks;

namespace KnittingApp
{
    public class User
    {
        public Guid Id { get; }                    
        public string Username { get; set; } = string.Empty;  
        public string Password { get; set; } = string.Empty;

        public readonly UserRepository userRepository;



        public User(Guid id, string username, string password)
        {
            Id = id;
            Username = username;
            Password = password;
        }

    }
}
