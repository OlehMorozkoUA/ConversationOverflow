using ConnectToDB;
using Microsoft.EntityFrameworkCore;
using Models.Classes;
using Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Classes.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ConversationOverflowDbContext _conversationOverflowDbContext;
        public GroupRepository(ConversationOverflowDbContext conversationOverflowDbContext)
        {
            _conversationOverflowDbContext = conversationOverflowDbContext;
        }

        public async Task AddGroupAsync(string login, Group group)
        {
            User user = _conversationOverflowDbContext.Users.AsQueryable().Where(user => user.Login == login).FirstOrDefaultAsync().Result;

            await _conversationOverflowDbContext.Groups.AddAsync(group);
            await _conversationOverflowDbContext.GroupUsers.AddAsync(new GroupUser()
            {
                UserId = user.Id,
                GroupId = group.Id
            });
        }

        public async Task<List<Group>> GetAllGroupsAsync(string login)
            => await _conversationOverflowDbContext.GroupUsers.AsQueryable()
            .Include(groupUser => groupUser.Group)
            .Include(groupUser => groupUser.User)
            .Where(groupUser => groupUser.User.Login == login)
            .Select(groupUser => groupUser.Group)
            .ToListAsync();

        public async Task<Group> GetGroupByIdAsync(int id)
            => await _conversationOverflowDbContext.Groups.FirstOrDefaultAsync(group => group.Id == id);

        public async Task Update(string login, int groupId, Group group)
        {
            User user = _conversationOverflowDbContext.Users.AsQueryable().Where(user => user.Login == login).FirstOrDefaultAsync().Result;

            
        }
    }
}
