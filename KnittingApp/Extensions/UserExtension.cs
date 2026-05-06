using KnittingApp.Models;

namespace KnittingApp.Extensions
{
    public static class UserExtension
    {
        public static UserModel ToModel(this User user)
        {
            UserModel model = new UserModel();
            model.UserId = user.Id;
            model.Username = user.Username;
            model.Password = user.Password;
            return model;
        }

        public static User ToObject(this UserModel model)
        {
            User user=new User(model.UserId, model.Username, model.Password);
            return user;
        }
    }
}
