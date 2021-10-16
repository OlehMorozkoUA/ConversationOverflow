using ConnectToDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Classes;
using Services.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Services.Classes.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ConversationOverflowDbContext _conversationOverflowDbContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public UserRepository(ConversationOverflowDbContext conversationOverflowDbContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _conversationOverflowDbContext = conversationOverflowDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
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

        public async Task<bool> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null) return false;
            User user = await _userManager.FindByIdAsync(userId);

            if (user == null) return false;
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded) return true;
            else return false;
        }

        public async Task<string> LogIn(string login, string password, bool rememberme, string returnUrl)
        {
            User user = await _userManager.FindByNameAsync(login);

            if (user == null) return "Користувача не існує!";
            if (!await _userManager.IsEmailConfirmedAsync(user)) return "Ви не підтвердили пошту!";

            SignInResult result =
                await _signInManager.PasswordSignInAsync(login, password, rememberme, false);

            if (result.Succeeded) return returnUrl;
            else return "Неправильний пароль або логін!";
        }

        public async Task LogOut()
            => await _signInManager.SignOutAsync();

        public async Task<List<User>> GetAllUserAsync()
            => await _conversationOverflowDbContext.Users.AsQueryable()
            .OrderBy(user => (user.FirstName + user.LastName))
            .ToListAsync();
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

    }
}
