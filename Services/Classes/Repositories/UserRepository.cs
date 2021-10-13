using ConnectToDB;
using Microsoft.EntityFrameworkCore;
using Models.Classes;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
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
        public async Task<User> CreateUserAsync(User user, string verificationCode)
        {
            string verfCode = "";
            bool isVerf = false;
            using (StreamReader read = new StreamReader(Path.Combine("Content", "Verification Code.csv")))
            {
                while (!read.EndOfStream)
                {
                    string line = await read.ReadLineAsync();
                    string[] values = line.Split(";");

                    if (values[0] == user.Email)
                    {
                        verfCode = values[1];
                        isVerf = true;
                    }
                }
            }
            bool isExist = await IsExistLogin(user.Login) || await IsExistEmail(user.Email);
            if (!isExist && isVerf && verfCode == verificationCode)
            {
                _conversationOverflowDbContext.Add(user);
                await _conversationOverflowDbContext.SaveChangesAsync();
            }

            return user;
        }

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

        public async Task SendVerificationCode(string email)
        {
            using (StreamReader read = new StreamReader(Path.Combine("Content", "Verification Code.csv")))
            {
                while (!read.EndOfStream)
                {
                    string line = await read.ReadLineAsync();
                    string[] values = line.Split(";");

                    if (values[0] == email) return;

                }
            }
            using (StreamWriter file = new StreamWriter(Path.Combine("Content", "Verification Code.csv"), true))
            {
                Guid guid = Guid.NewGuid();
                await file.WriteLineAsync(email + ";" + guid);
                await Sender.Send(email, "Verification Code", guid.ToString());
            }
        }
    }
}
