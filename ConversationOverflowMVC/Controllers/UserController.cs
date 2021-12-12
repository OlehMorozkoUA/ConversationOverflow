using ConversationOverflowMVC.Dto;
using ConversationOverflowMVC.Helper;
using Microsoft.AspNetCore.Mvc;
using Services.Classes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Models.Classes;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace ConversationOverflowMVC.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private HttpClient _httpClientConversationOverflowAPI;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConversationOverflowAPI _conversationOverflowAPI;
        [ViewData]
        public bool? IsAuthenticated { get; set; }
        [ViewData]
        public string AuthenticatedUser { get; set; }
        public UserController( IWebHostEnvironment hostingEnvironment,
               IHttpClientFactory httpClientFactory,
               IConversationOverflowAPI conversationOverflowAPI)
        {
            _conversationOverflowAPI = conversationOverflowAPI;
            _httpClientConversationOverflowAPI = conversationOverflowAPI.Initial();
            _hostingEnvironment = hostingEnvironment;
            AuthenticatedUser = User?.Identity?.Name;
            _httpClientFactory = httpClientFactory;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult LogIn(string message)
        {
            ViewData["Message"] = message;
            ViewData["Title"] = "LogIn";

            if (GetPasswordHash().Result == "") IsAuthenticated = false;
            else IsAuthenticated = User.Identity.IsAuthenticated;

            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Title"] = "Register";

            if (GetPasswordHash().Result == "") IsAuthenticated = false;
            else IsAuthenticated = User.Identity.IsAuthenticated;

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Privacy()
        {
            if (GetPasswordHash().Result == "") IsAuthenticated = false;
            else IsAuthenticated = User.Identity.IsAuthenticated;

            await ReloadHttpClient();
            if (User.Identity.IsAuthenticated)
            {
                ViewData["Title"] = "Edit";
                HttpResponseMessage httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("User/login/" + User.Identity.Name);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    User user = await httpResponseMessage.Content.ReadFromJsonAsync<User>();

                    if (user.Location == null) user.Location = new Location();

                    return View(user);
                }
                else return Redirect($"/User/LogIn?message=Залогуйтеся");
            }
            else return Redirect($"/User/LogIn?message=Залогуйтеся");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogInService([FromForm] LogInDto logInDto)
        {
            HttpResponseMessage httpResponseMessage = 
                await _httpClientConversationOverflowAPI.PostAsJsonAsync<LogInDto>("User/LogIn", logInDto);

            LogInMessage message;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                message = await httpResponseMessage.Content.ReadFromJsonAsync<LogInMessage>();
                
                if (message.IsSuccess)
                {
                    await Authenticate(logInDto.Login);
                    
                    return Redirect(message.Message);
                }
                else return Redirect($"/User/LogIn?message={message.Message}");
            }

            message = new LogInMessage() { IsSuccess = false, Message = "Запит не виконано, зверніться до адміністратора." };

            return Redirect($"/User/LogIn?message={message.Message}");
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterService([FromForm] RegisterDto registerDto)
        {
            HttpResponseMessage httpResponseMessage = 
                await _httpClientConversationOverflowAPI.PostAsJsonAsync<RegisterDto>("User/Register", registerDto);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string msg = await httpResponseMessage.Content.ReadAsStringAsync();
                if (msg == "true") return Redirect($"/User/LogIn?message=Перевірте електронну пошту {registerDto.Email}");
            }

            return Redirect($"/User/LogIn?message=Запит не виконано, зверніться до адміністратора.");
        }
        [HttpPost]
        public async Task<IActionResult> LogOutService()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect($"/User/LogIn");
        }
        [HttpGet]
        public async Task<IActionResult> List(int index = 0, int interval = 7)
        {
            if (GetPasswordHash().Result == "") IsAuthenticated = false;
            else IsAuthenticated = User.Identity.IsAuthenticated;

            if (ReloadHttpClient().Result)
            {
                ViewData["Title"] = "List";
                HttpResponseMessage httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("User/range/" + interval + "/" + index);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    List<User> users = await httpResponseMessage.Content.ReadFromJsonAsync<List<User>>();

                    httpResponseMessage = await _httpClientConversationOverflowAPI.GetAsync("User/countpagination/" + interval);

                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        string count = await httpResponseMessage.Content.ReadAsStringAsync();
                        ViewData["Count"] = count;

                        httpResponseMessage = await _httpClientConversationOverflowAPI.GetAsync("User/login/" + User.Identity.Name);
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            User user = await httpResponseMessage.Content.ReadFromJsonAsync<User>();

                            return View((users, user));
                        }
                        else return Redirect($"/User/LogIn?message={httpResponseMessage.StatusCode}");
                    }
                    else return Redirect($"/User/LogIn?message={httpResponseMessage.StatusCode}");
                }
                else return Redirect($"/User/LogIn?message={httpResponseMessage.StatusCode}");
            }
            else return Redirect($"/User/LogIn");
        }

        [HttpGet]
        public async Task<IActionResult> _User(int userId)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("User/" + userId);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    User user = await httpResponseMessage.Content.ReadFromJsonAsync<User>();

                    return PartialView("_User", user);
                }
                else return PartialView("_User", new User());
            }
            else return PartialView("_User", new User());
        } 

        [HttpGet]
        public async Task<string> GetLogin(int userId)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("User/" + userId);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    User user = await httpResponseMessage.Content.ReadFromJsonAsync<User>();

                    if (user != null) return user.Login;
                    else return "";
                }
                else return "";
            }
            else return "";
        }

        [HttpGet]
        public string GetCurrentLogin() => User.Identity.Name;

        [HttpGet]
        public async Task<List<int>> GetUserIds(int groupId)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("Group/userIds/" + groupId);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    List<int> userIds = await httpResponseMessage.Content.ReadFromJsonAsync<List<int>>();

                    if (userIds != null) return userIds;
                    else return new List<int>();
                }
                else return new List<int>();
            }
            else return new List<int>();
        }

        [HttpGet]
        public async Task<List<string>> GetUserLogins(int groupId)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("Group/userLogins/" + groupId);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    List<string> userLogins = await httpResponseMessage.Content.ReadFromJsonAsync<List<string>>();

                    if (userLogins != null) return userLogins;
                    else return new List<string>();
                }
                else return new List<string>();
            }
            else return new List<string>();
        }

        [HttpGet]
        public async Task<int> GetCountPagination(string name = "", int interval = 7)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage;

                if (name == "") httpResponseMessage =
                                    await _httpClientConversationOverflowAPI.GetAsync("User/countpagination/" + interval);
                else httpResponseMessage =
                         await _httpClientConversationOverflowAPI.GetAsync("User/countpagination/" + name + "/" + interval);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return Convert.ToInt32(await httpResponseMessage.Content.ReadAsStringAsync());
                }
                else return 0;
            }
            else return 0;
        }

        [HttpGet]
        public async Task<IActionResult> _ListUser(string name = "", int index = 0, int interval = 7, string userIds = "")
        {
            if (userIds == null) userIds = "";
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage;
                if (userIds == "")
                {
                    if (name == "") httpResponseMessage =
                        await _httpClientConversationOverflowAPI.GetAsync("User/range/" + interval + "/" + index);
                    else httpResponseMessage =
                            await _httpClientConversationOverflowAPI.GetAsync("User/rangebyname/" + name + "/" + interval + "/" + index);
                }
                else
                {
                    if (name == "") httpResponseMessage =
                        await _httpClientConversationOverflowAPI.GetAsync("User/rangeexcept/" + interval + "/" + userIds);
                    else httpResponseMessage =
                            await _httpClientConversationOverflowAPI.GetAsync("User/rangeexceptbyname/" + interval + "/" + name + "/" + userIds);
                }
                

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    List<User> users = await httpResponseMessage.Content.ReadFromJsonAsync<List<User>>();

                    return PartialView("_ListUser", (users, name));
                }
                else return PartialView("_ListUserError");
            }
            else return PartialView("_ListUserError");
        }

        [HttpGet]
        public async Task<int> GetCountUserPagination(string name = "", int interval = 7)
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
        public async Task<IActionResult> _Pagination(string name = "", int index = 0, int interval = 7)
        {
            await ReloadHttpClient();
            int max = await GetCountPagination(name, interval);

            if (index >= 0)
            {
                index++;
                (int index, int max, string name) tuple = (index, max, name);
                return PartialView("_Pagination", tuple);
            }
            else return PartialView("_Pagination", (1, max, name));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            if (GetPasswordHash().Result == "") IsAuthenticated = false;
            else IsAuthenticated = User.Identity.IsAuthenticated;

            ViewData["Title"] = "Forgot Password";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordService([FromForm] ForgotPasswordDto forgotPasswordDto)
        {
            if (GetPasswordHash().Result == "") IsAuthenticated = false;
            else IsAuthenticated = User.Identity.IsAuthenticated;

            ViewData["Title"] = "Forgot Password";
            ViewData["Email"] = forgotPasswordDto.Email;

            HttpResponseMessage httpResponseMessage =
                await _httpClientConversationOverflowAPI.PostAsJsonAsync<ForgotPasswordDto>("User/ForgotPassword", forgotPasswordDto);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string result = await httpResponseMessage.Content.ReadAsStringAsync();

                return View();
            }
            else return Redirect($"/User/LogIn?message=Залогуйтеся");
        }

        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            if (GetPasswordHash().Result == "") IsAuthenticated = false;
            else IsAuthenticated = User.Identity.IsAuthenticated;

            ViewData["code"] = code;
            return View();
        }

        public async Task<string> ResetPasswordService([FromForm]ResetPasswordDto resetPasswordDto)
        {
            HttpResponseMessage httpResponseMessage =
                await _httpClientConversationOverflowAPI.PostAsJsonAsync<ResetPasswordDto>("User/ChangePassword", resetPasswordDto);

            return await httpResponseMessage.Content.ReadAsStringAsync();

            //if (httpResponseMessage.IsSuccessStatusCode)
            //{
            //    string result = await httpResponseMessage.Content.ReadAsStringAsync();

            //    if(Convert.ToBoolean(result)) return Redirect($"/User/LogIn?message=Пароль змінено.");
            //    else return Redirect($"/User/LogIn?message=Запит не виконано, зверніться до адміністратора.");
            //}
            //else return Redirect($"/User/LogIn?message=Запит не виконано, зверніться до адміністратора.");
        }

        [HttpPost]
        public async Task<bool> UpdateFirstName([FromForm] string firstname)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                        await _httpClientConversationOverflowAPI.PutAsync("User/UpdateFirstName",
                                    new StringContent(JsonSerializer.Serialize(new { firstname = firstname }), Encoding.UTF8, "application/json"));

                return httpResponseMessage.IsSuccessStatusCode;
            }
            else return false;
        }

        [HttpPost]
        public async Task<bool> UpdateLastName([FromForm] string lastname)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                        await _httpClientConversationOverflowAPI.PutAsync("User/UpdateLastName",
                                    new StringContent(JsonSerializer.Serialize(new { lastname = lastname }), Encoding.UTF8, "application/json"));

                return httpResponseMessage.IsSuccessStatusCode;
            }
            else return false;
        }

        [HttpPost]
        public async Task<bool> UpdateBirthday([FromForm] DateTime birthday)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                        await _httpClientConversationOverflowAPI.PutAsync("User/UpdateBirthday",
                                    new StringContent(JsonSerializer.Serialize(new { birthday = birthday.ToString("yyyy-M-d") }), Encoding.UTF8, "application/json"));

                return httpResponseMessage.IsSuccessStatusCode;
            }
            else return false;
        }

        [HttpPost]
        public async Task<bool> UpdateImage([FromForm] IFormFile image)
        {
            if (ReloadHttpClient().Result)
            {
                if (image != null)
                {
                    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "content");
                    string fileName = User.Identity.Name + Path.GetExtension(image.FileName);
                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var form = new MultipartFormDataContent())
                    {
                        using (var fs = image.OpenReadStream())
                        {
                            using (var streamContent = new StreamContent(fs))
                            {
                                using (var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync()))
                                {
                                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                                    form.Add(fileContent, "image", Path.GetFileName(filePath));
                                    form.Add(new StringContent(filePath, Encoding.UTF8, "text/xml"), "filepath");

                                    HttpResponseMessage httpResponseMessage = await _httpClientConversationOverflowAPI.PutAsync("User/UpdateImage", form);

                                    return httpResponseMessage.IsSuccessStatusCode;
                                }
                            }
                        }
                    }
                }
                else return false;
            }
            else return false;
        }

        [HttpPost]
        public async Task<bool> UpdatePhoneNumber([FromForm] string phonenumber)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                        await _httpClientConversationOverflowAPI.PutAsync("User/UpdatePhoneNumber",
                                    new StringContent(JsonSerializer.Serialize(new { phonenumber = phonenumber }), Encoding.UTF8, "application/json"));

                return httpResponseMessage.IsSuccessStatusCode;
            }
            else return false;
        }

        [HttpPost]
        public async Task<bool> UpdateLocation([FromForm] LocationDto locationDto)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                await _httpClientConversationOverflowAPI.PutAsJsonAsync("User/UpdateLocation", locationDto);

                return httpResponseMessage.IsSuccessStatusCode;
            }
            else return false;
        }

        [HttpGet]
        public async Task<int> GetUserId()
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                        await _httpClientConversationOverflowAPI.GetAsync("User/login/" + User.Identity.Name);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    User user = await httpResponseMessage.Content.ReadFromJsonAsync<User>();

                    return user.Id;
                }
                else return 0;
            }
            else return 0;
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "User")
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
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