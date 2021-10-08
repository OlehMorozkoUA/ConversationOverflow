using ConnectToDB;
using Microsoft.EntityFrameworkCore;
using Models.Classes;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
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
            _conversationOverflowDbContext.Add(user);
            await _conversationOverflowDbContext.SaveChangesAsync();

            return user;
        }

        public async Task<List<User>> GetAllUserAsync()
        {
            return await _conversationOverflowDbContext.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _conversationOverflowDbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        }
    }
}
