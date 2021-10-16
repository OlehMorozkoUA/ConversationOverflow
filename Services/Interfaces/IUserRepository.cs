using Microsoft.AspNetCore.Identity;
using Models.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserRepository
    {
        Task<IdentityResult> CreateUserAsync(User user, string password);
        Task<string> GenerateEmailConfirmationTokenAsync(User user);
        Task SendEmailAsync(User user, string callbackUrl);
        Task<bool> ConfirmEmail(string userId, string code);
        Task<string> LogIn(string login, string password, bool rememberme, string returnUrl);
        Task LogOut();
        Task<List<User>> GetAllUserAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByLoginAsync(string login);
        Task<User> GetUserByEmailAsync(string email);
        Task<List<User>> GetUserByNameAsync(string name);
        Task<List<User>> GetUsersByBirthdayAsync(string birthday);
    }
}
