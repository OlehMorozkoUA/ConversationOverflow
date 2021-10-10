using Models.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUserAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<User> GetUserByLoginAsync(string login);
        Task<User> GetUserByEmailAsync(string email);
        Task<List<User>> GetUserByNameAsync(string name);
    }
}
