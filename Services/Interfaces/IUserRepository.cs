using Models.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user, string verificationCode);
        Task<List<User>> GetAllUserAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByLoginAsync(string login);
        Task<User> GetUserByEmailAsync(string email);
        Task<List<User>> GetUserByNameAsync(string name);
        Task<List<User>> GetUsersByBirthdayAsync(string birthday);
        Task SendVerificationCode(string email);
    }
}
