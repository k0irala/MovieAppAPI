using MovieApplicationApi.Models.Entities;

namespace MovieApplicationApi.Repository
{
    public interface IUserRepository
    {
        User GetUserById(int id);
        int AddUser(User user);
        int UpdateUser(int id, User user);
        int DeleteUser(int id);
        List<User> GetAllUsers();                                
    }
}
