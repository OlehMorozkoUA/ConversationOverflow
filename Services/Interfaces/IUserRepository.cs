using Microsoft.AspNetCore.Identity;
using Models.Classes;
using Services.Classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserRepository
    {
        Task<IdentityResult> CreateUserAsync(User user, string password);
        Task<string> GenerateEmailConfirmationTokenAsync(User user);
        Task SendEmailAsync(User user, string callbackUrl);
        Task<List<User>> GetAllUserAsync();
        Task<List<User>> GetRangeUserAsync(int interval, int index);
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByLoginAsync(string login);
        Task<User> GetUserByEmailAsync(string email);
        Task<List<User>> GetUserByNameAsync(string name);
        Task<List<User>> GetUsersByBirthdayAsync(string birthday);
        Task UpdateFirstName(string login, string firstname);
        Task UpdateLastName(string login, string lastname);
        Task UpdateBirthday(string login, DateTime birthday);
        Task UpdateImagePath(string login, string imagepath);
        Task UpdatePhoneNumber(string login, string phonenumber);
        Task<int> GetCountUserPaginationAsync(int interval);
        Task<int> GetCountUserPaginationAsync(string name, int interval);
        Task<List<User>> GetRangeUserByNameAsync(string name, int interval, int index);
    }
}
