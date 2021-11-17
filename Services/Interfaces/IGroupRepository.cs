using Models.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IGroupRepository
    {
        Task AddGroupAsync(string login, Group group);
        Task<List<Group>> GetAllGroupsAsync(string login);
        Task<Group> GetGroupByIdAsync(int id);
        Task Update(string login, int groupId, Group group);
    }
}
