using ConnectToDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Classes;
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
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IServiceProvider _service;
        private readonly ConversationOverflowDbContext _conversationOverflowDbContext;

        public UserController(ILogger<WeatherForecastController> logger, IServiceProvider service)
        {
            _logger = logger;
            _service = service;
            _conversationOverflowDbContext = service.GetService(typeof(ConversationOverflowDbContext)) as ConversationOverflowDbContext;
        }

        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _conversationOverflowDbContext.Users;
        }
        [HttpGet("{id}")]
        public User Get(int id)
        {
            return _conversationOverflowDbContext.Users.FirstOrDefault(user => user.Id == id);
        }
        [HttpGet]
        [Route("Find/{login}")]
        public User Get(string login)
        {
            return _conversationOverflowDbContext.Users.FirstOrDefault(user => user.Login == login);
        }
        /*[HttpGet("{login}")]
        public User GetUserByLogin(string login)
        {
            return _conversationOverflowDbContext.Users.FirstOrDefault(user => user.Login == login);
        }
        [HttpGet("{email}")]
        public User GetUserByEmail(string email)
        {
            return _conversationOverflowDbContext.Users.FirstOrDefault(user => user.Email == email);
        }*/
    }
}
