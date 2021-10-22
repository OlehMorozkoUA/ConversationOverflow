using ConversationOverflow.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Classes;
using Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Services.Classes;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mime;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace ConversationOverflow.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _users;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserController(ILogger<UserController> logger, 
            IUserRepository users,
            IConfiguration configuration,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _logger = logger;
            _users = users;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [Route("[action]")]
        [AllowAnonymous]
        public bool IsAuthenticated() => User.Identity.IsAuthenticated;

        [HttpGet]
        [Route("[action]")]
        [AllowAnonymous]
        public string AuthenticatedUser() => User.Identity.Name;

        [HttpGet]
        public async Task<List<User>> Get() => await _users.GetAllUserAsync();

        [HttpGet]
        [Route("range/{interval}/{index}")]
        public async Task<List<User>> GetRange(int interval, int index) => await _users.GetRangeUserAsync(interval, index);

        [HttpGet("{id:int}")]
        public async Task<User> GetById(int id) => await _users.GetUserByIdAsync(id);

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
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<bool> Register([FromForm] RegisterDto registerDto)
        {
            User user = new User()
            {
                Login = registerDto.Login,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.Login,
                Birthday = registerDto.Birthday
            };

            IdentityResult result = await _users.CreateUserAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                string code = await _users.GenerateEmailConfirmationTokenAsync(user);
                await _users.SendEmailAsync(user, Url.Action(
                    "ConfirmEmail",
                    "User",
                    new { userId = user.Id, code = code },
                    protocol: HttpContext.Request.Scheme));

                return true;
            }
            else return false;
        }

        [HttpGet]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<bool> ConfirmEmail(string userId, string code)
            => await _users.ConfirmEmail(userId, code);

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<LogInMessage> LogIn([FromBody] LogInDto logInDto)
        {
            return await _users.LogIn(logInDto.Login, logInDto.Password, logInDto.RememberMe, logInDto.ReturnUrl);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task LogOut() => await _users.LogOut();
    }
}
