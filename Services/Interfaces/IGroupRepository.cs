using Models.Classes;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IGroupRepository
    {
        Task AddGroupAsync(Group group);
        Task<Group> GetGroupByIdAsync(int id);
    }
}
