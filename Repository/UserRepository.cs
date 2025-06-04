using Dapper;
using WebApplication1.Interfaces;
using WebApplication1.Models.Entities;

namespace MovieApplicationApi.Repository
{
    public class UserRepository(IDapperRepository repository) : IUserRepository
    {
        public int AddUser(User user)
        {
            DynamicParameters parameters = new();
            parameters.Add("@userName", user.Username);
            parameters.Add("@phone", user.Phone);
            parameters.Add("@address", user.Address);
            parameters.Add("@email", user.Email);
            parameters.Add("@password", user.Password);
            parameters.Add("@role", user.Roles);    
            parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
            
            repository.Execute("sp_AddUserProc", parameters);

            int result = parameters.Get<int>("@Result");
            return result;
        }

        public int DeleteUser(int id)
        {
            DynamicParameters parameters = new();
            parameters.Add("@userId", id);
            parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            repository.Execute("sp_DeleteUser", parameters);

            int result = parameters.Get<int>("@Result");
            return result;
        }

        public List<User> GetAllUsers()
        {
            DynamicParameters parameters = new();
            var users = repository.Query<User>("sp_GetAllUsers",parameters);
            return [.. users];
        }

        public User GetUserById(int id)
        {
            DynamicParameters parameters = new();
            parameters.Add("@USERId", id);
            var user = repository.QuerySingleOrDefault<User>("sp_GetUserById", parameters);
            return user ?? throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        public int UpdateUser(int id, User user)
        { 
            DynamicParameters parameters = new();
            parameters.Add("@Id", id);
            parameters.Add("@Username", user.Username);
            parameters.Add("@Phone", user.Phone);
            parameters.Add("@Address", user.Address);
            parameters.Add("@Email", user.Email);
            parameters.Add("@Password", user.Password);
            parameters.Add("@Roles", 1);    
            parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
            repository.Execute("sp_UpdateUser", parameters);
            int result = parameters.Get<int>("@Result");
            return result;      
        }
    }
}
