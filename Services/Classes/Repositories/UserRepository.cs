using ConnectToDB;
using Microsoft.EntityFrameworkCore;
using Models.Classes;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.Classes.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ConversationOverflowDbContext _conversationOverflowDbContext;
        public UserRepository(ConversationOverflowDbContext conversationOverflowDbContext)
        {
            _conversationOverflowDbContext = conversationOverflowDbContext;
        }
        public async Task<User> CreateUserAsync(User user)
        {
            bool isExist = await IsExistLogin(user.Login) && await IsExistEmail(user.Email);
            if (isExist)
            {
                _conversationOverflowDbContext.Add(user);
                await _conversationOverflowDbContext.SaveChangesAsync();
            }
            
            return user;
        }

        public async Task<List<User>> GetAllUserAsync()
            => await _conversationOverflowDbContext.Users.ToListAsync();
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
        
        /*public async Task<List<User>> GetUsersAsync(System.Linq.Expressions.Expression<Func<List<User>,bool>> predicate, System.Threading.CancellationToken cancellationToken = default)
        {
            return await _conversationOverflowDbContext.Users.FindAsync
        }*/
    }
}
