using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConversationOverflowMVC.Helper
{
    public class ConversationOverflowAPI : IConversationOverflowAPI
    {
        private HttpClient httpClient;
        public ConversationOverflowAPI()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:5001");
        }
        public HttpClient Initial() => httpClient;
        public async Task<string> IsAuthenticated()
        {
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("User/IsAuthenticated");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string isAuthenticated = await httpResponseMessage.Content.ReadAsStringAsync();
                return isAuthenticated;
            }
            else return "false";
        }
        public async Task<string> AuthenticatedUser()
        {
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("User/AuthenticatedUser");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string isAuthenticated = await httpResponseMessage.Content.ReadAsStringAsync();
                return isAuthenticated;
            }
            else return "";
        }
    }
}
