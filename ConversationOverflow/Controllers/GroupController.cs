using ConversationOverflow.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Classes;
using Services.Classes;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConversationOverflow.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IGroupRepository _groups;
        private readonly IUserRepository _users;

        public GroupController(ILogger<UserController> logger,
            IGroupRepository groups,
            IUserRepository users)
        {
            _logger = logger;
            _groups = groups;
            _users = users;
        }

        [HttpGet]
        public async Task<List<Group>> Get() => await _groups.GetAllGroupsAsync(User.Identity.Name);
        [HttpGet]
        [Route("{id}")]
        public async Task<Group> GetByGroupId(int id) => await _groups.GetGroupByIdAsync(User.Identity.Name, id);
        [HttpGet]
        [Route("users/{groupId}")]
        public async Task<List<User>> GetUsers(int groupId) => await _groups.GetUsers(User.Identity.Name, groupId);
        [HttpGet]
        [Route("userIds/{groupId}")]
        public async Task<List<int>> GetUserIds(int groupId) => await _groups.GetUserIds(User.Identity.Name, groupId);
        [HttpGet]
        [Route("userLogins/{groupId}")]
        public async Task<List<string>> GetUserLogins(int groupId) => await _groups.GetUserLogins(User.Identity.Name, groupId);
        [HttpGet]
        [Route("existprivate/{userId}")]
        public async Task<ExistPrivateGroupDto> IsExistPrivateGroup(int userId) => await _groups.IsExistPrivateGroup(User.Identity.Name, userId);
        [HttpGet]
        [Route("onlygroup/{interval}/{index}")]
        public async Task<List<Group>> GetRangeOnlyGroup(int interval, int index)
            => await _groups.GetRangeOnlyGroupAsync(User.Identity.Name, interval, index);
        [HttpGet]
        [Route("onlygroup/{name}/{interval}/{index}")]
        public async Task<List<Group>> GetRangeOnlyGroup(string name, int interval, int index)
            => await _groups.GetRangeOnlyGroupAsync(User.Identity.Name, name, interval, index);
        [HttpGet]
        [Route("range/{interval}/{index}")]
        public async Task<List<Group>> GetRange(int interval, int index) => await _groups.GetRangeGroupAsync(User.Identity.Name, interval, index);
        [HttpGet]
        [Route("userinprivategroup/{groupId}")]
        public async Task<User> GetUserInPrivateGroup(int groupId) => await _groups.GetUserInPrivateGroup(User.Identity.Name, groupId);
        [HttpGet]
        [Route("rangebyname/{name}/{interval}/{index}")]
        public async Task<List<Group>> GetRangeByName(string name, int interval, int index)
            => await _groups.GetRangeGroupByNameAsync(User.Identity.Name, name, interval, index);
        [HttpGet]
        [Route("countpagination/{interval}")]
        public async Task<int> GetCountPagination(int interval) => await _groups.GetCountGroupPaginationAsync(User.Identity.Name, interval);

        [HttpGet]
        [Route("countpagination/{name}/{interval}")]
        public async Task<int> GetCountPagination(string name, int interval) => await _groups.GetCountGroupPaginationAsync(User.Identity.Name, name, interval);
        [HttpGet]
        [Route("countonlygrouppagination/{interval}")]
        public async Task<int> GetCountOnlyGroupPagination(int interval) => await _groups.GetCountOnlyGroupPaginationAsync(User.Identity.Name, interval);

        [HttpGet]
        [Route("countonlygrouppagination/{name}/{interval}")]
        public async Task<int> GetCountOnlyGroupPagination(string name, int interval) => await _groups.GetCountOnlyGroupPaginationAsync(User.Identity.Name, name, interval);
        [HttpGet]
        [Route("[action]/{name}/{adminId}/{dateTime}")]
        public async Task<Group> GetGroupByNameAndAdminId(string name, int adminId, DateTime dateTime) => await _groups.GetGroupByNameAndAdminIdAsync(User.Identity.Name, name, adminId, dateTime);
        [HttpGet]
        [Route("[action]/{groupId}")]
        public async Task<bool> IsPrivateGroup(int groupId) => await _groups.IsPrivateGroupAsync(User.Identity.Name, groupId);
        [HttpPost]
        public async Task<Group> Post([FromBody] Group group)
        {
            User user = _users.GetUserByLoginAsync(User.Identity.Name).Result;
            group.AdminId = user.Id;
            Group newGroup = _groups.AddGroupAsync(User.Identity.Name, group).Result;

            await _groups.AddUserToGroup(user.Login, newGroup.Id, user.Id);

            return newGroup;
        }
        [HttpPost]
        [Route("[action]")]
        public async Task AddUserToGroup([FromBody] GroupUserDto groupUserDto)
            => await _groups.AddUserToGroup(User.Identity.Name, groupUserDto.GroupId, groupUserDto.UserId);

        [HttpPost]
        [Route("[action]")]
        public async Task<Group> AddPrivateGroup([FromBody] UserIdDto userIdDto)
            => await _groups.AddPrivateGroupAsync(User.Identity.Name, userIdDto.UserId);
    }
}
