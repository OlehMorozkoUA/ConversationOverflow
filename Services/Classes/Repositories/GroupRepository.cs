using ConnectToDB;
using Microsoft.EntityFrameworkCore;
using Models.Classes;
using Services.Interfaces;
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

        public async Task AddGroupAsync(Group group)
            => await _conversationOverflowDbContext.AddAsync(group);

        public async Task<Group> GetGroupByIdAsync(int id)
            => await _conversationOverflowDbContext.Groups.FirstOrDefaultAsync(group => group.Id == id);
    }
}
