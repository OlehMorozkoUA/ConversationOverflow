using ConversationOverflowMVC.Dto;
using ConversationOverflowMVC.Helper;
using Microsoft.AspNetCore.Mvc;
using Services.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ConversationOverflowMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClientConversationOverflowAPI;
        private readonly IWebHostEnvironment _hostingEnvironment;
        [ViewData]
        public bool IsAuthenticated { get; set; }
        [ViewData]
        public string AuthenticatedUser { get; set; }
        public UserController(IConversationOverflowAPI conversationOverflowAPI,
               IWebHostEnvironment hostingEnvironment)
        {
            _httpClientConversationOverflowAPI = conversationOverflowAPI.Initial();
            _hostingEnvironment = hostingEnvironment;
            IsAuthenticated = Convert.ToBoolean(conversationOverflowAPI.IsAuthenticated().Result);
            AuthenticatedUser = conversationOverflowAPI.AuthenticatedUser().Result;
        }
        [HttpGet]
        public IActionResult LogIn(string message)
        {
            ViewData["Message"] = message;
            ViewData["Title"] = "LogIn";
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Title"] = "Register";
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Privacy()
        {
            if (IsAuthenticated)
            {
                ViewData["Title"] = "Edit";
                HttpResponseMessage httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("User/login/" + AuthenticatedUser);

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
        public async Task<IActionResult> LogInService([FromForm] LogInDto logInDto)
        {
            HttpResponseMessage httpResponseMessage = 
                await _httpClientConversationOverflowAPI.PostAsJsonAsync<LogInDto>("User/LogIn", logInDto);
            LogInMessage message;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                message = await httpResponseMessage.Content.ReadFromJsonAsync<LogInMessage>();

                if (message.IsSuccess) return Redirect(message.Message);
                else return Redirect($"/User/LogIn?message={message.Message}");
            }

            message = new LogInMessage() { IsSuccess = false, Message = "Запит не виконано, зверніться до адміністратора." };

            return Redirect($"/User/LogIn?message={message.Message}");
        }
        [HttpPost]
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
            await _httpClientConversationOverflowAPI.PostAsync("User/LogOut", null);

            return Redirect($"/User/LogIn");
        }
        [HttpGet]
        public async Task<IActionResult> List(int index = 0, int interval = 7)
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
                    return View(users);
                }
                else return Redirect($"/User/LogIn?message={httpResponseMessage.StatusCode}");
            }
            else return Redirect($"/User/LogIn?message={httpResponseMessage.StatusCode}");
        }

        [HttpGet]
        public async Task<int> GetCountPagination(string name = "", int interval = 7)
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

        [HttpGet]
        public async Task<IActionResult> _ListUser(string name = "", int index = 0, int interval = 7)
        {
            HttpResponseMessage httpResponseMessage;

            if (name == "") httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("User/range/" + interval + "/" + index);
            else httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("User/rangebyname/" + name + "/" + interval + "/" + index);


            if (httpResponseMessage.IsSuccessStatusCode)
            {
                List<User> users = await httpResponseMessage.Content.ReadFromJsonAsync<List<User>>();

                return PartialView("_ListUser", users);
            }
            else return Redirect($"/User/LogIn?message={httpResponseMessage.StatusCode}");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            ViewData["Title"] = "Forgot Password";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordService([FromForm] ForgotPasswordDto forgotPasswordDto)
        {
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
            HttpResponseMessage httpResponseMessage =
                        await _httpClientConversationOverflowAPI.PutAsync("User/UpdateFirstName", 
                                    new StringContent(JsonSerializer.Serialize(new { firstname = firstname }), Encoding.UTF8, "application/json"));

            return httpResponseMessage.IsSuccessStatusCode;
        }

        [HttpPost]
        public async Task<bool> UpdateLastName([FromForm] string lastname)
        {
            HttpResponseMessage httpResponseMessage =
                        await _httpClientConversationOverflowAPI.PutAsync("User/UpdateLastName",
                                    new StringContent(JsonSerializer.Serialize(new { lastname = lastname }), Encoding.UTF8, "application/json"));

            return httpResponseMessage.IsSuccessStatusCode;
        }

        [HttpPost]
        public async Task<bool> UpdateBirthday([FromForm] DateTime birthday)
        {
            HttpResponseMessage httpResponseMessage =
                        await _httpClientConversationOverflowAPI.PutAsync("User/UpdateBirthday",
                                    new StringContent(JsonSerializer.Serialize(new { birthday = birthday.ToString("yyyy-M-d") }), Encoding.UTF8, "application/json"));

            return httpResponseMessage.IsSuccessStatusCode;
        }

        [HttpPost]
        public async Task<bool> UpdateImage([FromForm] IFormFile image)
        {
            if (image != null)
            {
                string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "content");
                string fileName = AuthenticatedUser + Path.GetExtension(image.FileName);
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

        [HttpPost]
        public async Task<bool> UpdatePhoneNumber([FromForm] string phonenumber)
        {
            HttpResponseMessage httpResponseMessage =
                        await _httpClientConversationOverflowAPI.PutAsync("User/UpdatePhoneNumber",
                                    new StringContent(JsonSerializer.Serialize(new { phonenumber = phonenumber }), Encoding.UTF8, "application/json"));

            return httpResponseMessage.IsSuccessStatusCode;
        }

        [HttpPost]
        public async Task<bool> UpdateLocation([FromForm] LocationDto locationDto)
        {
            HttpResponseMessage httpResponseMessage =
                await _httpClientConversationOverflowAPI.PutAsJsonAsync("User/UpdateLocation", locationDto);

            return httpResponseMessage.IsSuccessStatusCode;
        }
    }
}