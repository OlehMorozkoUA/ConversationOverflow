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

namespace ConversationOverflowMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClientConversationOverflowAPI;
        [ViewData]
        public bool IsAuthenticated { get; set; }
        [ViewData]
        public string AuthenticatedUser { get; set; }
        public UserController(IConversationOverflowAPI conversationOverflowAPI)
        {
            _httpClientConversationOverflowAPI = conversationOverflowAPI.Initial();
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
        public async Task<IActionResult> List(int index = 0, int interval = 10)
        {
            ViewData["Title"] = "List";
            HttpResponseMessage httpResponseMessage = 
                await _httpClientConversationOverflowAPI.GetAsync("User/range/" + interval + "/" + index);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                List<User> users = await httpResponseMessage.Content.ReadFromJsonAsync<List<User>>();

                return View(users);
            }
            else return Redirect($"/User/LogIn?message={httpResponseMessage.StatusCode}");
        }

        [HttpGet]
        public async Task<IActionResult> _ListUser(int index = 0, int interval = 10)
        {
            HttpResponseMessage httpResponseMessage = 
                await _httpClientConversationOverflowAPI.GetAsync("User/range/" + interval + "/" + index);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                List<User> users = await httpResponseMessage.Content.ReadFromJsonAsync<List<User>>();

                return PartialView("_ListUser", users);
            }
            else return Redirect($"/User/LogIn?message={httpResponseMessage.StatusCode}");
            
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
    }
}