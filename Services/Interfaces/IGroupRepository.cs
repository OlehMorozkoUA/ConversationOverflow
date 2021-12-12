using Models.Classes;
using Services.Classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IGroupRepository
    {
        Task<Group> AddGroupAsync(string login, Group group);
        Task<Group> AddPrivateGroupAsync(string login, int userId);
        Task<List<Group>> GetAllGroupsAsync(string login);
        Task<List<Group>> GetRangeGroupAsync(string login, int interval, int index);
        Task<User> GetUserInPrivateGroup(string login, int groupId);
        Task<List<Group>> GetRangeGroupByNameAsync(string login, string name, int interval, int index);
        Task<int> GetCountGroupPaginationAsync(string login, int interval);
        Task<int> GetCountGroupPaginationAsync(string login, string name, int interval);
        Task<int> GetCountOnlyGroupPaginationAsync(string login, int interval);
        Task<int> GetCountOnlyGroupPaginationAsync(string login, string name, int interval);
        Task<Group> GetGroupByIdAsync(string login, int id);
        Task<List<Group>> GetRangeOnlyGroupAsync(string login, int interval, int index);
        Task<List<Group>> GetRangeOnlyGroupAsync(string login, string name, int interval, int index);
        Task<List<User>> GetUsers(string login, int groupId);
        Task<List<int>> GetUserIds(string login, int groupId);
        Task<List<string>> GetUserLogins(string login, int groupId);
        Task<ExistPrivateGroupDto> IsExistPrivateGroup(string login, int userId);
        Task<bool> IsPrivateGroupAsync(string login, int groupId);
        Task<Group> GetGroupByNameAndAdminIdAsync(string login, string name, int adminId, DateTime dateTime);
        Task UpdateName(string login, int groupId, string name);
        Task AddUserToGroup(string login, int groupId, int userId);
    }
}
