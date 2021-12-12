using ConnectToDB;
using Microsoft.EntityFrameworkCore;
using Models.Classes;
using Services.Interfaces;
using System;
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

        public async Task<Group> AddGroupAsync(string login, Group group)
        {
            User user = _conversationOverflowDbContext.Users.AsQueryable()
                .Where(user => user.Login == login)
                .FirstOrDefaultAsync().Result;
            Group groupCheck = _conversationOverflowDbContext.GroupUsers.AsQueryable()
                .Include(groupUser => groupUser.Group)
                .Where(groupUser => groupUser.UserId == user.Id)
                .Select(groupUser => groupUser.Group)
                .Where(g => g.Name == group.Name)
                .FirstOrDefaultAsync().Result;

            if (groupCheck == null)
            {
                if (group.ImagePath == null) group.ImagePath = "/content/Group_default.png";
                _conversationOverflowDbContext.Groups.Add(group);
                _conversationOverflowDbContext.SaveChanges();

                GroupUser groupUser = new GroupUser()
                {
                    GroupId = group.Id,
                    UserId = user.Id
                };
                _conversationOverflowDbContext.GroupUsers.Add(groupUser);
                await _conversationOverflowDbContext.SaveChangesAsync();
                return group;
            }
            else return new Group();
        }
        public async Task<Group> AddPrivateGroupAsync(string login, int userId)
        {
            User user = _conversationOverflowDbContext.Users.AsQueryable()
                .Where(user => user.Login == login)
                .FirstOrDefaultAsync().Result;
            if (user != null)
            {
                List<int> privateGroupIds = await _conversationOverflowDbContext.GroupUsers.AsQueryable()
                    .Include(gu => gu.Group)
                    .Include(gu => gu.User)
                    .Where(gu => gu.Group.IsPrivate && gu.User.Login == login)
                    .Select(gu => gu.GroupId)
                    .ToListAsync();

                if (privateGroupIds != null)
                {
                    Group group = await _conversationOverflowDbContext.GroupUsers.AsQueryable()
                        .Include(gu => gu.Group)
                        .Where(gu => privateGroupIds.Contains(gu.Group.Id) && gu.UserId == userId)
                        .Select(gu => gu.Group)
                        .FirstOrDefaultAsync();

                    if (group == null)
                    {
                        group = new Group()
                        {
                            CreationTime = DateTime.Now,
                            IsPrivate = true,
                            ImagePath = "/content/Group_default.png"
                        };
                        _conversationOverflowDbContext.Groups.Add(group);
                        await _conversationOverflowDbContext.SaveChangesAsync();

                        GroupUser groupUser = new GroupUser()
                        {
                            UserId = user.Id,
                            GroupId = group.Id
                        };
                        _conversationOverflowDbContext.GroupUsers.Add(groupUser);
                        await _conversationOverflowDbContext.SaveChangesAsync();

                        groupUser = new GroupUser()
                        {
                            UserId = userId,
                            GroupId = group.Id
                        };
                        _conversationOverflowDbContext.GroupUsers.Add(groupUser);
                        await _conversationOverflowDbContext.SaveChangesAsync();

                        return group;
                    }
                    else return group;
                }
                else return new Group();
            }
            else return new Group();
        }

        public async Task AddUserToGroup(string login, int groupId, int userId)
        {
            User user = _conversationOverflowDbContext.Users.AsQueryable().Where(user => user.Login == login).FirstOrDefaultAsync().Result;
            if (user != null)
            {
                Group group = await _conversationOverflowDbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
                if (group != null)
                {
                    GroupUser groupUser = await _conversationOverflowDbContext.GroupUsers.AsQueryable()
                        .Where(g => g.GroupId == groupId && g.UserId == user.Id)
                        .FirstOrDefaultAsync();
                    if (groupUser != null)
                    {
                        GroupUser groupUserCheck = await _conversationOverflowDbContext.GroupUsers.AsQueryable()
                            .Where(g => g.GroupId == groupId && g.UserId == userId)
                            .FirstOrDefaultAsync();
                        User userCheck = _conversationOverflowDbContext.Users.AsQueryable().Where(user => user.Id == userId).FirstOrDefaultAsync().Result;
                        if (groupUserCheck == null && userCheck != null)
                        {
                            _conversationOverflowDbContext.GroupUsers.Add(new GroupUser()
                            {
                                UserId = userId,
                                GroupId = groupId
                            });
                            await _conversationOverflowDbContext.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        public async Task<List<Group>> GetAllGroupsAsync(string login)
            => await GetGroupQueryable(login)
            .ToListAsync();

        public async Task<Group> GetGroupByIdAsync(string login, int id)
            => await _conversationOverflowDbContext.GroupUsers.AsQueryable()
            .Include(gu => gu.Group)
            .Include(gu => gu.User)
            .Where(gu => gu.User.Login == login)
            .Select(gu => gu.Group).FirstOrDefaultAsync(group => group.Id == id);

        public async Task<List<Group>> GetRangeOnlyGroupAsync(string login, int interval, int index)
            => await _conversationOverflowDbContext.GroupUsers.AsQueryable()
            .Include(gu => gu.Group)
            .Include(gu => gu.User)
            .Where(gu => gu.User.Login == login && !gu.Group.IsPrivate)
            .Select(gu => gu.Group)
            .Skip(index * interval)
            .Take(interval)
            .ToListAsync();

        public async Task<List<Group>> GetRangeOnlyGroupAsync(string login, string name, int interval, int index)
            => await _conversationOverflowDbContext.GroupUsers.AsQueryable()
            .Include(gu => gu.Group)
            .Include(gu => gu.User)
            .Where(gu => gu.User.Login == login && !gu.Group.IsPrivate && (
                gu.Group.Name.StartsWith(name.Trim().ToLower()) &&
                gu.Group.Name.Contains(name.Trim().ToLower())
            ))
            .Select(gu => gu.Group)
            .Skip(index * interval)
            .Take(interval)
            .ToListAsync();

        public async Task<List<Group>> GetRangeGroupAsync(string login, int interval, int index)
        {
            List<Group> groups = await GetRangeGroupQueryable(login, interval, index).ToListAsync();
            ChangeNamePrivateGroups(login, groups);

            return groups;
        }

        private void ChangeNamePrivateGroups(string login, List<Group> groups)
        {
            groups = groups.Where(group => group.IsPrivate).ToList();
            foreach(Group group in groups)
            {
                User user = GetUserInPrivateGroup(login, group.Id).Result;
                group.Name = user.FirstName + " " + user.LastName;
                group.ImagePath = user.ImagePath;
            }
        }

        protected IQueryable<Group> GetRangeGroupQueryable(string login, int interval, int index)
            => _conversationOverflowDbContext.GroupUsers.AsQueryable()
            .Include(gu => gu.Group)
            .Include(gu => gu.User)
            .Where(gu => gu.User.Login == login)
            .OrderBy(gu => gu.Group.Name)
            .Select(gu => gu.Group)
            .Skip(index * interval)
            .Take(interval);

        public async Task<User> GetUserInPrivateGroup(string login, int groupId)
            => await GetUserInPrivateGroupQueryable(login, groupId)
            .FirstOrDefaultAsync();

        protected IQueryable<User> GetUserInPrivateGroupQueryable(string login, int groupId)
        {
            bool permission = (_conversationOverflowDbContext.GroupUsers.AsQueryable()
                .Include(gu => gu.User)
                .Where(gu => gu.User.Login == login && gu.GroupId == groupId && gu.Group.IsPrivate).Count() > 0);
            if (permission)
            {
                return _conversationOverflowDbContext.GroupUsers.AsQueryable()
                    .Include(gu => gu.Group)
                    .Include(gu => gu.User)
                    .Where(gu => gu.User.Login != login && gu.GroupId == groupId && gu.Group.IsPrivate)
                    .Select(gu => gu.User);
            }
            else return _conversationOverflowDbContext.GroupUsers.AsQueryable()
                    .Where(gu => 1 != 1)
                    .Select(gu => gu.User);
        }

        public async Task<List<Group>> GetRangeGroupByNameAsync(string login, string name, int interval, int index)
        {
            List<Group> groups = await GetGroupByName(login, name).ToListAsync();
            List<Group> privateGroups = groups
                .Where(g => g.IsPrivate)
                .ToList();
            foreach (Group group in privateGroups)
            {
                User user = GetUserInPrivateGroup(login, group.Id).Result;
                group.Name = user.FirstName + " " + user.LastName;
                group.ImagePath = user.ImagePath;
            }
            groups = groups.Where(group => group.Name.ToLower().StartsWith(name.Trim().ToLower()) &&
                                           group.Name.ToLower().Contains(name.Trim().ToLower())).ToList();

            return groups
                .Skip(index * interval)
                .Take(interval)
                .ToList(); 
        }

        protected IQueryable<Group> GetGroupByName(string login, string name)
            => _conversationOverflowDbContext.GroupUsers.AsQueryable()
            .Include(gu => gu.Group)
            .Include(gu => gu.User)
            .Where(gu => gu.User.Login == login &&
                                        (
                                            (gu.Group.Name.StartsWith(name.Trim()) &&
                                             gu.Group.Name.Contains(name.Trim())) ||
                                            (gu.Group.IsPrivate)
                                        )
            )
            .OrderBy(gu => gu.Group.Name)
            .Select(gu => gu.Group);

        protected IQueryable<Group> GetRangeGroupByNameQueryable(string login, string name, int interval, int index)
            => _conversationOverflowDbContext.GroupUsers.AsQueryable()
            .Include(gu => gu.Group)
            .Include(gu => gu.User)
            .Where(gu => gu.User.Login == login && 
                                        (
                                            (gu.Group.Name.StartsWith(name.Trim()) && 
                                             gu.Group.Name.Contains(name.Trim())) ||
                                            (gu.Group.IsPrivate)
                                        )
            )
            .OrderBy(gu => gu.Group.Name)
            .Select(gu => gu.Group)
            .Skip(index * interval)
            .Take(interval);

        public async Task<int> GetCountGroupPaginationAsync(string login, int interval)
            => Convert.ToInt32(
                Math.Ceiling((await GetGroupQueryable(login).CountAsync()) / Convert.ToDouble(interval)));

        public async Task<int> GetCountGroupPaginationAsync(string login, string name, int interval)
            => Convert.ToInt32(
                Math.Ceiling((await GetCountGroupByName(login, name)) / Convert.ToDouble(interval)));

        public async Task<int> GetCountOnlyGroupPaginationAsync(string login, int interval)
            => Convert.ToInt32(
                Math.Ceiling((await GetOnlyGroupQueryable(login).CountAsync()) / Convert.ToDouble(interval)));

        public async Task<int> GetCountOnlyGroupPaginationAsync(string login, string name, int interval)
            => Convert.ToInt32(
                Math.Ceiling((await GetOnlyGroupByNameQueryable(login, name).CountAsync()) / Convert.ToDouble(interval)));



        protected IQueryable<Group> GetGroupByNameQueryable(string login, string name)
            => _conversationOverflowDbContext.GroupUsers.AsQueryable()
            .Include(gu => gu.Group)
            .Include(gu => gu.User)
            .Where(gu => gu.User.Login == login && (
                                                    (gu.Group.Name.StartsWith(name.Trim()) && 
                                                     gu.Group.Name.Contains(name.Trim())) ||
                                                    (gu.Group.IsPrivate)
                                                   )
            )
            .OrderBy(gu => gu.Group.Name)
            .Select(gu => gu.Group);

        public async Task<int> GetCountGroupByName(string login, string name)
        {
            List<Group> groups = await GetGroupByName(login, name).ToListAsync();
            List<Group> privateGroups = groups
                .Where(g => g.IsPrivate)
                .ToList();
            foreach (Group group in privateGroups)
            {
                User user = GetUserInPrivateGroup(login, group.Id).Result;
                group.Name = user.FirstName + " " + user.LastName;
                group.ImagePath = user.ImagePath;
            }
            groups = groups.Where(group => group.Name.ToLower().StartsWith(name.Trim().ToLower()) &&
                                           group.Name.ToLower().Contains(name.Trim().ToLower())).ToList();

            return groups.Count();
        }

        protected IQueryable<Group> GetGroupQueryable(string login)
            => _conversationOverflowDbContext.GroupUsers.AsQueryable()
            .Include(gu => gu.Group)
            .Include(gu => gu.User)
            .Where(gu => gu.User.Login == login)
            .OrderBy(gu => gu.Group.Name)
            .Select(gu => gu.Group);

        protected IQueryable<Group> GetOnlyGroupQueryable(string login)
            => _conversationOverflowDbContext.GroupUsers.AsQueryable()
            .Include(gu => gu.Group)
            .Include(gu => gu.User)
            .Where(gu => gu.User.Login == login && !gu.Group.IsPrivate)
            .OrderBy(gu => gu.Group.Name)
            .Select(gu => gu.Group);

        protected IQueryable<Group> GetOnlyGroupByNameQueryable(string login, string name)
            => _conversationOverflowDbContext.GroupUsers.AsQueryable()
            .Include(gu => gu.Group)
            .Include(gu => gu.User)
            .Where(gu => gu.User.Login == login && !gu.Group.IsPrivate && 
            (
                gu.Group.Name.ToLower().StartsWith(name.Trim().ToLower()) &&
                gu.Group.Name.ToLower().Contains(name.Trim().ToLower())
            ))
            .OrderBy(gu => gu.Group.Name)
            .Select(gu => gu.Group);

        public async Task<List<User>> GetUsers(string login, int groupId)
        {
            User user = await _conversationOverflowDbContext.GroupUsers.AsQueryable()
                            .Include(gu => gu.Group)
                            .Include(gu => gu.User)
                            .Where(gu => gu.User.Login == login && gu.GroupId == groupId)
                            .Select(gu => gu.User)
                            .FirstOrDefaultAsync();
            if (user != null)
            {
                List<int> userIds = await _conversationOverflowDbContext.GroupUsers.AsQueryable()
                            .Include(gu => gu.Group)
                            .Include(gu => gu.User)
                            .Where(gu => gu.GroupId == groupId)
                            .Select(gu => gu.UserId)
                            .ToListAsync();

                List<User> users = await _conversationOverflowDbContext.Users.AsQueryable()
                    .Include(u => u.Location)
                    .Where(u => userIds.Contains(u.Id) && u.Id != user.Id)
                    .ToListAsync();

                return users;
            }
            else return new List<User>();
        }

        public async Task<List<int>> GetUserIds(string login, int groupId)
        {
            User user = await _conversationOverflowDbContext.GroupUsers.AsQueryable()
                            .Include(gu => gu.Group)
                            .Include(gu => gu.User)
                            .Where(gu => gu.User.Login == login && gu.GroupId == groupId)
                            .Select(gu => gu.User)
                            .FirstOrDefaultAsync();
            if (user != null)
            {
                List<int> userIds = await _conversationOverflowDbContext.GroupUsers.AsQueryable()
                            .Where(gu => gu.GroupId == groupId)
                            .Select(gu => gu.UserId)
                            .ToListAsync();

                return userIds;
            }
            else return new List<int>();
        }

        public async Task<List<string>> GetUserLogins(string login, int groupId)
        {
            User user = await _conversationOverflowDbContext.GroupUsers.AsQueryable()
                            .Include(gu => gu.Group)
                            .Include(gu => gu.User)
                            .Where(gu => gu.User.Login == login && gu.GroupId == groupId)
                            .Select(gu => gu.User)
                            .FirstOrDefaultAsync();
            if (user != null)
            {
                List<string> userLogins = await _conversationOverflowDbContext.GroupUsers.AsQueryable()
                            .Include(gu => gu.User)
                            .Where(gu => gu.GroupId == groupId)
                            .Select(gu => gu.User.Login)
                            .ToListAsync();

                return userLogins;
            }
            else return new List<string>();
        }

        public async Task<ExistPrivateGroupDto> IsExistPrivateGroup(string login, int userId)
        {
            List<int> groupIds = await _conversationOverflowDbContext.GroupUsers.AsQueryable()
                .Include(gu => gu.Group)
                .Include(gu => gu.User)
                .Where(gu => gu.User.Login == login && gu.Group.IsPrivate)
                .Select(gu => gu.GroupId)
                .ToListAsync();

            ExistPrivateGroupDto existPrivateGroupDto;
            for (int i = 0; i < groupIds.Count; i++)
            {
                List<User> users = await GetUsers(login, groupIds[i]);
                if (users.Find(u => u.Id == userId) != null)
                {
                    existPrivateGroupDto = new ExistPrivateGroupDto()
                    {
                        GroupId = groupIds[i],
                        Result = true
                    };
                    return existPrivateGroupDto;
                }
            }
            existPrivateGroupDto = new ExistPrivateGroupDto()
            {
                GroupId = 0,
                Result = false
            };
            return existPrivateGroupDto;
        }

        public async Task<bool> IsPrivateGroupAsync(string login, int groupId)
        {
            User user = await _conversationOverflowDbContext.GroupUsers.AsQueryable()
                            .Include(gu => gu.Group)
                            .Include(gu => gu.User)
                            .Where(gu => gu.User.Login == login && gu.GroupId == groupId && gu.Group.IsPrivate)
                            .Select(gu => gu.User)
                            .FirstOrDefaultAsync();

            return user != null;
        }

        public async Task<Group> GetGroupByNameAndAdminIdAsync(string login, string name, int adminId, DateTime dateTime)
            => await _conversationOverflowDbContext.GroupUsers.AsQueryable()
            .Include(gu => gu.Group)
            .Include(gu => gu.User)
            .Where(gu => gu.User.Login == login && 
                        !gu.Group.IsPrivate && 
                         gu.Group.AdminId == adminId &&
                         gu.Group.CreationTime == dateTime
            )
            .OrderBy(gu => gu.Group.Name)
            .Select(gu => gu.Group)
            .FirstOrDefaultAsync();

        public async Task UpdateName(string login, int groupId, string name)
        {
            User user = _conversationOverflowDbContext.Users.AsQueryable().Where(user => user.Login == login).FirstOrDefaultAsync().Result;
            if (user != null)
            {
                GroupUser groupUser = await _conversationOverflowDbContext.GroupUsers.AsQueryable()
                    .Where(g => g.Id == groupId && g.UserId == user.Id)
                    .FirstOrDefaultAsync();
                if (groupUser != null)
                {
                    Group group = await _conversationOverflowDbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId);

                    group.Name = name;
                    _conversationOverflowDbContext.SaveChanges();
                }
            }
        }
    }
}
