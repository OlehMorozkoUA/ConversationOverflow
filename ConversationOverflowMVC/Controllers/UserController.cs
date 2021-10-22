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
        private readonly IConversationOverflowAPI _conversationOverflowAPI;
        [ViewData]
        public bool IsAuthenticated { get; set; }
        [ViewData]
        public string AuthenticatedUser { get; set; }
        public UserController(IConversationOverflowAPI conversationOverflowAPI)
        {
            _conversationOverflowAPI = conversationOverflowAPI;
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
        [HttpPost]
        public async Task<IActionResult> LogInService([FromForm] LogInDto logInDto)
        {
            HttpResponseMessage httpResponseMessage = await _httpClientConversationOverflowAPI.PostAsJsonAsync<LogInDto>("User/LogIn", logInDto);
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
        public async Task<IActionResult> LogOutService()
        {
            await _httpClientConversationOverflowAPI.PostAsync("User/LogOut", null);

            return Redirect($"/User/LogIn");
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            ViewData["Title"] = "List";
            HttpResponseMessage httpResponseMessage = await _httpClientConversationOverflowAPI.GetAsync("User");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                List<User> users = await httpResponseMessage.Content.ReadFromJsonAsync<List<User>>();

                return View(users);
            }
            else return Redirect($"/User/LogIn?message={httpResponseMessage.StatusCode}");
        }
    }
}
