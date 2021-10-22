using System.Net.Http;
using System.Threading.Tasks;

namespace ConversationOverflowMVC.Helper
{
    public interface IConversationOverflowAPI
    {
        public HttpClient Initial();
        public Task<string> IsAuthenticated();
        public Task<string> AuthenticatedUser();
    }
}
