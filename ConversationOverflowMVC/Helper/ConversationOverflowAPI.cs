using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConversationOverflowMVC.Helper
{
    public class ConversationOverflowAPI : IConversationOverflowAPI
    {
        private readonly HttpClient _httpClient;
        public ConversationOverflowAPI(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:5001");
            _httpClient.Timeout = TimeSpan.FromSeconds(3000);
        }
        public HttpClient Initial() => _httpClient;
        public async Task<string> IsAuthenticated()
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("User/IsAuthenticated");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string isAuthenticated = await httpResponseMessage.Content.ReadAsStringAsync();
                return isAuthenticated;
            }
            else return "false";
        }
        public async Task<string> AuthenticatedUser()
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("User/AuthenticatedUser");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string isAuthenticated = await httpResponseMessage.Content.ReadAsStringAsync();
                return isAuthenticated;
            }
            else return "";
        }
    }
}
