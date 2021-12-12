using ConversationOverflowMVC.Dto;
using ConversationOverflowMVC.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Classes;
using Services.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ConversationOverflowMVC.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        private readonly HttpClient _httpClientConversationOverflowAPI;
        private readonly IConversationOverflowAPI _conversationOverflowAPI;
        [ViewData]
        public bool IsAuthenticated { get; set; }
        [ViewData]
        public string AuthenticatedUser { get; set; }

        public GroupController(IConversationOverflowAPI conversationOverflowAPI)
        {
            _conversationOverflowAPI = conversationOverflowAPI;
            _httpClientConversationOverflowAPI = conversationOverflowAPI.Initial();
            IsAuthenticated = Convert.ToBoolean(conversationOverflowAPI.IsAuthenticated().Result);
            AuthenticatedUser = conversationOverflowAPI.AuthenticatedUser().Result;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            if (ReloadHttpClient().Result)
            {
                ViewData["Title"] = "Groups";
                return View();
            }
            else return Redirect($"/User/LogIn?message=Залогуйтеся");
        }

        [HttpGet]
        public async Task<IActionResult> _ListGroupCard(string name = "", int index = 0, int interval = 3)
        {
            await ReloadHttpClient();
            HttpResponseMessage httpResponseMessage;

            if (name == "") httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("Group/onlygroup/" + interval + "/" + index);
            else httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("Group/onlygroup/" + name + "/" + interval + "/" + index);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                List<Group> groups = await httpResponseMessage.Content.ReadFromJsonAsync<List<Group>>();
                Dictionary<int, List<User>> users = new Dictionary<int, List<User>>();
                foreach(Group group in groups)
                {
                    httpResponseMessage =
                        await _httpClientConversationOverflowAPI.GetAsync("Group/users/" + group.Id);

                    List<User> listUser = await httpResponseMessage.Content.ReadFromJsonAsync<List<User>>();

                    users.Add(group.Id, listUser);
                }

                return PartialView("_ListGroupCard", (groups, users, name));
            }
            else return PartialView("_ListGroupCard", (new List<Group>(), new Dictionary<int, List<User>>(), name));
        }

        public async Task<IActionResult> Add()
        {
            if (ReloadHttpClient().Result)
            {
                ViewData["Title"] = "Add Group";
                return View();
            }
            else return Redirect($"/User/LogIn?message=Залогуйтеся");
        }

        [HttpGet]
        public async Task<IActionResult> _ListGroup(string name = "", int index = 0, int interval = 7)
        {
            await ReloadHttpClient();
            HttpResponseMessage httpResponseMessage;

            if (name == "") httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("Group/range/" + interval + "/" + index);
            else httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("Group/rangebyname/" + name + "/" + interval + "/" + index);


            if (httpResponseMessage.IsSuccessStatusCode)
            {
                List<Group> groups = await httpResponseMessage.Content.ReadFromJsonAsync<List<Group>>();

                return PartialView("_ListGroup", (groups, name));
            }
            else
            {
                List<Group> groups = new List<Group>();

                return PartialView("_ListGroup", (groups, name));
            }
        }

        [HttpGet]
        public async Task<IActionResult> _User(int groupId)
        {
            await ReloadHttpClient();
            HttpResponseMessage httpResponseMessage =
                await _httpClientConversationOverflowAPI.GetAsync("Group/userinprivategroup/" + groupId);
            (Group group, User user) groupUser;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                try
                {
                    groupUser.user = await httpResponseMessage.Content.ReadFromJsonAsync<User>(); 
                }
                catch (Exception) 
                {
                    groupUser = (new Group(), new User());
                }
            }
            else return PartialView("_User", (new Group(), new User()));

            httpResponseMessage =
                        await _httpClientConversationOverflowAPI.GetAsync("Group/" + groupId);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                groupUser.group = await httpResponseMessage.Content.ReadFromJsonAsync<Group>();
            }
            else return PartialView("_User", (new Group(), new User()));

            return PartialView("_User", groupUser);
        }
        [HttpGet]
        public async Task<ExistPrivateGroupDto> IsExistPrivateGroup(int userId)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("Group/existprivate/" + userId);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    ExistPrivateGroupDto existPrivateGroupDto = await httpResponseMessage.Content.ReadFromJsonAsync<ExistPrivateGroupDto>();

                    return existPrivateGroupDto;
                }
                else return new ExistPrivateGroupDto()
                {
                    GroupId = 0,
                    Result = false
                };
            }
            else return new ExistPrivateGroupDto()
            {
                GroupId = 0,
                Result = false
            };
        }

        [HttpGet]
        public async Task<int> GetCountPagination(string name = "", int interval = 7)
        {
            await ReloadHttpClient();
            HttpResponseMessage httpResponseMessage;

            if (name == "") httpResponseMessage =
                                await _httpClientConversationOverflowAPI.GetAsync("Group/countpagination/" + interval);
            else httpResponseMessage =
                     await _httpClientConversationOverflowAPI.GetAsync("Group/countpagination/" + name + "/" + interval);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return Convert.ToInt32(await httpResponseMessage.Content.ReadAsStringAsync());
            }
            else return 0;
        }

        [HttpGet]
        public async Task<int> GetCountOnlyGroupPagination(string name = "", int interval = 7)
        {
            await ReloadHttpClient();
            HttpResponseMessage httpResponseMessage;

            if (name == "") httpResponseMessage =
                                await _httpClientConversationOverflowAPI.GetAsync("Group/countonlygrouppagination/" + interval);
            else httpResponseMessage =
                     await _httpClientConversationOverflowAPI.GetAsync("Group/countonlygrouppagination/" + name + "/" + interval);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return Convert.ToInt32(await httpResponseMessage.Content.ReadAsStringAsync());
            }
            else return 0;
        }

        [HttpGet]
        public async Task<IActionResult> _PaginationGroup(string name = "", int index = 0, int interval = 7)
        {
            await ReloadHttpClient();
            int max = await GetCountOnlyGroupPagination(name, interval);

            if (index >= 0)
            {
                index++;
                (int index, int max, string name) tuple = (index, max, name);
                return PartialView("_PaginationGroup", tuple);
            }
            else return PartialView("_PaginationGroup", (1, max, name));
        }

        [HttpPost]
        public async Task<bool> AddService(string name, List<int> userIds)
        {
            await ReloadHttpClient();
            Group group = new Group()
            {
                Name = name,
                CreationTime = DateTime.Now
            };
            HttpResponseMessage httpResponseMessage =
                await _httpClientConversationOverflowAPI.PostAsJsonAsync<Group>("Group", group);

            group = await httpResponseMessage.Content.ReadFromJsonAsync<Group>();

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                foreach (int userId in userIds)
                {
                    GroupUserDto groupUserDto = new GroupUserDto()
                    {
                        GroupId = group.Id,
                        UserId = userId
                    };
                    httpResponseMessage =
                        await _httpClientConversationOverflowAPI.PostAsJsonAsync<GroupUserDto>("Group/AddUserToGroup", groupUserDto);
                }
            }
            
            return httpResponseMessage.IsSuccessStatusCode;
        }

        [HttpGet]
        public async Task<bool> IsPrivateGroup(int groupId)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                await _httpClientConversationOverflowAPI.GetAsync("Group/IsPrivateGroup/" + groupId);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    bool result = await httpResponseMessage.Content.ReadFromJsonAsync<bool>();

                    return result;
                }
                else return false;
            }
            else return false;
        }

        private async Task<bool> ReloadHttpClient()
        {
            UserDto userDto = new UserDto()
            {
                Login = User?.Identity?.Name,
                PasswordHash = GetPasswordHash().Result
            };
            HttpResponseMessage httpResponseMessage =
                await _httpClientConversationOverflowAPI.PostAsJsonAsync<UserDto>("User/ReloadHttpClient", userDto);

            return httpResponseMessage.IsSuccessStatusCode;
        }

        private async Task<string> GetPasswordHash()
        {
            HttpResponseMessage httpResponseMessage =
                await _httpClientConversationOverflowAPI.GetAsync("User/login/" + User?.Identity?.Name);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                User user = await httpResponseMessage.Content.ReadFromJsonAsync<User>();

                if (user.Location == null) user.Location = new Location();

                return user.PasswordHash;
            }

            return "";
        }
    }
}
