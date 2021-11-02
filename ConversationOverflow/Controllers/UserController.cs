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
using System.Text.Json;
using System;
using System.IO;

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

        [HttpGet]
        [Route("rangebyname/{name}/{interval}/{index}")]
        public async Task<List<User>> GetRangeByName(string name, int interval, int index)
            => await _users.GetRangeUserByNameAsync(name, interval, index);

        [HttpGet]
        [Route("countpagination/{interval}")]
        public async Task<int> GetCountPagination(int interval) => await _users.GetCountUserPaginationAsync(interval);

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
        public async Task<bool> Register([FromBody] RegisterDto registerDto)
        {
            User user = new User()
            {
                Login = registerDto.Login,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.Login,
                Birthday = registerDto.Birthday,
                ImagePath = "/content/User_default.svg"
            };

            IdentityResult result = await _users.CreateUserAsync(user, registerDto.Password);
            if (result != null)
            {
                if (result.Succeeded)
                {
                    string code = await _users.GenerateEmailConfirmationTokenAsync(user);
                    await _users.SendEmailAsync(user, Url.Action(
                        "ConfirmEmail",
                        "User",
                        new { userId = user.Id, code = code, returnUrl = registerDto.ReturnUrl },
                        protocol: HttpContext.Request.Scheme));

                    return true;
                }
                else return false;
            }
            else return false;
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<bool> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            User user = await _users.GetUserByEmailAsync(forgotPasswordDto.Email);
            string code = await _users.GenerateEmailConfirmationTokenAsync(user);
            await _users.SendEmailAsync(user, Url.Action(
                "ResetPassword",
                "User",
                new { userId = user.Id, code = code, returnUrl = forgotPasswordDto.ReturnUrl },
                protocol: HttpContext.Request.Scheme));

            return true;
        }

        [HttpGet]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string userId, string code, string returnUrl)
        {
            if (userId == null || code == null) Redirect(returnUrl.Replace("/User/ResetPassword", "/User/LogIn"));
            User user = await _userManager.FindByIdAsync(userId);

            if (user == null) return Redirect(returnUrl.Replace("/User/ResetPassword", "/User/LogIn"));
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded) return Redirect(returnUrl + "?"
                + System.Net.WebUtility.UrlEncode("code") + "="
                + System.Net.WebUtility.UrlEncode(code));
            else return Redirect(returnUrl.Replace("/User/ResetPassword", "/User/LogIn"));
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<string> ChangePassword([FromBody]ResetPasswordDto resetPasswordDto)
        {
            User user = await _users.GetUserByEmailAsync(resetPasswordDto.Email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Code, resetPasswordDto.Password);
                return result.Succeeded.ToString() + ", " + resetPasswordDto.Code + ", " + resetPasswordDto.Password;
            }
            return "user null";
        }

        [HttpGet]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code, string returnUrl)
        {
            if (userId == null || code == null) return RedirectPermanent(returnUrl + "?"
                + System.Net.WebUtility.UrlEncode("message") + "="
                + System.Net.WebUtility.UrlEncode("пошта непідтверджена."));
            User user = await _userManager.FindByIdAsync(userId);

            if (user == null) return RedirectPermanent(returnUrl + "?"
                + System.Net.WebUtility.UrlEncode("message") + "="
                + System.Net.WebUtility.UrlEncode("пошта непідтверджена."));
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded) return RedirectPermanent(returnUrl + "?"
                + System.Net.WebUtility.UrlEncode("message") + "="
                + System.Net.WebUtility.UrlEncode("пошта підтверджена."));
            else return RedirectPermanent(returnUrl + "?"
                + System.Net.WebUtility.UrlEncode("message") + "="
                + System.Net.WebUtility.UrlEncode("пошта непідтверджена."));
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<LogInMessage> LogIn([FromBody] LogInDto logInDto)
        {
            User user = await _userManager.FindByNameAsync(logInDto.Login);

            if (user == null) return new LogInMessage() { IsSuccess = false, Message = "Користувача не існує!" };
            if (!await _userManager.IsEmailConfirmedAsync(user)) return new LogInMessage() { IsSuccess = false, Message = "Ви не підтвердили пошту!" };

            Microsoft.AspNetCore.Identity.SignInResult result =
                await _signInManager.PasswordSignInAsync(logInDto.Login, logInDto.Password, logInDto.RememberMe, false);

            if (result.Succeeded) return new LogInMessage() { IsSuccess = true, Message = logInDto.ReturnUrl };
            else return new LogInMessage() { IsSuccess = false, Message = "Неправильний пароль або логін!" };
        }

        [HttpPost]
        [Route("[action]")]
        public async Task LogOut()
            => await _signInManager.SignOutAsync();

        [HttpPut]
        [Route("[action]")]
        public async Task UpdateFirstName([FromBody] JsonElement body)
            => await _users.UpdateFirstName(User.Identity.Name, body.GetProperty("firstname").ToString());

        [HttpPut]
        [Route("[action]")]
        public async Task UpdateLastName([FromBody] JsonElement body)
            => await _users.UpdateLastName(User.Identity.Name, body.GetProperty("lastname").ToString());

        [HttpPut]
        [Route("[action]")]
        public async Task UpdateBirthday([FromBody] JsonElement body)
        {
            DateTime date = DateTime.ParseExact(body.GetProperty("birthday").ToString(), 
                "yyyy-M-d", System.Globalization.CultureInfo.InvariantCulture);
            await _users.UpdateBirthday(User.Identity.Name, date);
        }

        [HttpPut]
        [Route("[action]")]
        public async Task UpdateImage([FromForm] FileDto fileDto)
        {
            string folderPath = fileDto.FilePath.Replace(Path.GetFileName(fileDto.FilePath), "");
            string[] filePaths = Directory.GetFiles(folderPath);
            foreach (string fp in filePaths)
                if (fp.Contains(User.Identity.Name)) System.IO.File.Delete(fp);

            using (var file = new FileStream(fileDto.FilePath, FileMode.Create))
            {
                await fileDto.Image.CopyToAsync(file);
                await _users.UpdateImagePath(User.Identity.Name, "/content/" + Path.GetFileName(fileDto.FilePath));
            }
        }

        [HttpPut]
        [Route("[action]")]
        public async Task UpdatePhoneNumber([FromBody] JsonElement body)
            => await _users.UpdatePhoneNumber(User.Identity.Name, body.GetProperty("phonenumber").ToString());
    }
}