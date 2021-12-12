using Models.Classes;
using Services.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> AddMessageAsync(Message message);
        Task<UserMessage> AddUserMessageAsync(UserMessage userMessage);
        Task<bool> Send(string login, UserMessage userMessage, Message message, Attachment attachment);
        Task<List<MessageDto>> GetMessageAsync(string login, int groupId);
        Task<List<MessageDto>> GetMessageRangeAsync(string login, int groupId, int interval, int index);
        Task<List<MessageDto>> GetMessageRangeAtAsync(string login, int groupId, int startId, int count);
        Task<Attachment> GetAttachmentByMessageIdAsync(string login, int groupId, int messageId);
        Task<bool> IsGroup(string login, int groupId);
    }
}
