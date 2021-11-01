using ConnectToDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Classes;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Services.Classes.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ConversationOverflowDbContext _conversationOverflowDbContext;
        private readonly UserManager<User> _userManager;
        public UserRepository(ConversationOverflowDbContext conversationOverflowDbContext,
            UserManager<User> userManager)
        {
            _conversationOverflowDbContext = conversationOverflowDbContext;
            _userManager = userManager;
        }
        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            bool isExist = await IsExistLogin(user.Login) || await IsExistEmail(user.Email);
            if (isExist) return null;

            IdentityResult result = await _userManager.CreateAsync(user, password);
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "User"));

            return result;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
            => await _userManager.GenerateEmailConfirmationTokenAsync(user);

        public async Task SendEmailAsync(User user, string callbackUrl)
        {
            EmailService emailService = new EmailService();
            await emailService.SendEmailAsync(user.Email,
                "Confirm your accout",
                $"Підтвердіть реєстрацію, перейдіть за посиланням: <a href='{callbackUrl}'>link</a>");
        }

        public async Task<List<User>> GetAllUserAsync()
            => await _conversationOverflowDbContext.Users.AsQueryable()
            .OrderBy(user => (user.FirstName + user.LastName))
            .ToListAsync();
        public async Task<List<User>> GetRangeUserAsync(int interval, int index)
            => await _conversationOverflowDbContext.Users.AsQueryable()
            .OrderBy(user => (user.FirstName + user.LastName))
            .Skip(index * interval)
            .Take(interval)
            .ToListAsync();

        public async Task<int> GetCountUserPaginationAsync(int interval)
            => Convert.ToInt32(
                Math.Ceiling((await _conversationOverflowDbContext.Users.AsQueryable().CountAsync()) / Convert.ToDouble(interval)));

        public async Task<User> GetUserByIdAsync(int id)
            => await _conversationOverflowDbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        public async Task<User> GetUserByLoginAsync(string login)
            => await _conversationOverflowDbContext.Users.FirstOrDefaultAsync(user => user.Login == login);
        public async Task<User> GetUserByEmailAsync(string email)
            => await _conversationOverflowDbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
        public async Task<List<User>> GetUserByNameAsync(string name)
            => await _conversationOverflowDbContext.Users.AsQueryable()
                .Where(user => (user.FirstName.Contains(name.Trim()) && user.FirstName.StartsWith(name.Trim())) || 
                               (user.LastName.Contains(name.Trim()) && user.LastName.StartsWith(name.Trim())) ||
                               ((user.FirstName + " " + user.LastName).Contains(name.Trim()) && (user.FirstName + " " + user.LastName).StartsWith(name.Trim())) ||
                               ((user.LastName + " " + user.FirstName).Contains(name.Trim()) && (user.LastName + " " + user.FirstName).StartsWith(name.Trim())))
                .OrderBy(user => user.FirstName)
                .ToListAsync(); 
        public async Task<List<User>> GetUsersByBirthdayAsync(string birthday)
            => await _conversationOverflowDbContext.Users.AsQueryable()
                .Where(user => (user.Birthday.Year.ToString()+"-"+
                                user.Birthday.Month.ToString()+"-"+
                                user.Birthday.Day.ToString()) == birthday)
                .OrderBy(user => user.Birthday)
                .ToListAsync();
        public async Task<bool> IsExistLogin(string login)
            => (await GetUserByLoginAsync(login) != null) ? true : false;
        public async Task<bool> IsExistEmail(string email)
            => (await GetUserByEmailAsync(email) != null) ? true : false;

        public async Task UpdateFirstName(string login, string firstname)
        {
            User user = await _conversationOverflowDbContext.Users.AsQueryable()
            .Where(user => user.Login == login).FirstOrDefaultAsync();

            user.FirstName = firstname;
            await _conversationOverflowDbContext.SaveChangesAsync();
        }

        public async Task UpdateLastName(string login, string lastname)
        {
            User user = await _conversationOverflowDbContext.Users.AsQueryable()
            .Where(user => user.Login == login).FirstOrDefaultAsync();

            user.LastName = lastname;
            await _conversationOverflowDbContext.SaveChangesAsync();
        }

        public async Task UpdateBirthday(string login, DateTime birthday)
        {
            User user = await _conversationOverflowDbContext.Users.AsQueryable()
            .Where(user => user.Login == login).FirstOrDefaultAsync();

            user.Birthday = birthday;
            await _conversationOverflowDbContext.SaveChangesAsync();
        }

        public async Task UpdateImagePath(string login, string imagepath)
        {
            User user = await _conversationOverflowDbContext.Users.AsQueryable()
            .Where(user => user.Login == login).FirstOrDefaultAsync();

            user.ImagePath = imagepath;
            await _conversationOverflowDbContext.SaveChangesAsync();
        }

        public async Task UpdatePhoneNumber(string login, string phonenumber)
        {
            User user = await _conversationOverflowDbContext.Users.AsQueryable()
            .Where(user => user.Login == login).FirstOrDefaultAsync();

            user.PhoneNumber = phonenumber;
            await _conversationOverflowDbContext.SaveChangesAsync();
        }
    }
}