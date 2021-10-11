using ConnectToDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Classes;
using Services.Classes.Repositories;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConversationOverflow.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _users;

        public UserController(ILogger<UserController> logger, IServiceProvider service, IUserRepository users)
        {
            _logger = logger;
            _users = users;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> Get() => await _users.GetAllUserAsync();

        [HttpGet("{id:int}")]
        public async Task<User> Get(int id) => await _users.GetUserByIdAsync(id);

        [HttpGet]
        [Route("login/{login}")]
        public async Task<User> GetByLogin(string login) => await _users.GetUserByLoginAsync(login);

        [HttpGet]
        [Route("email/{email}")]
        public async Task<User> GetByEmail(string email) => await _users.GetUserByLoginAsync(email);

        [HttpGet]
        [Route("name/{name}")]
        public async Task<List<User>> GetByName(string name) => await _users.GetUserByNameAsync(name);
        [HttpGet]
        [Route("birthday/{birthday}")]
        public async Task<List<User>> GetByBirthday(string birthday) => await _users.GetUsersByBirthdayAsync(birthday);
        [HttpPost]
        [Route("registrate")]
        public async Task<User> CreateUser(string login, string password, string email, string firstname, string lastname, DateTime birthday)
        {
            User user = new User()
            {
                Login = login,
                Password = password,
                Email = email,
                FirstName = firstname,
                LastName = lastname,
                Birthday = birthday,
                Status = Models.Interfaces.Status.User
            };
            return await _users.CreateUserAsync(user);
        }
    }
}
