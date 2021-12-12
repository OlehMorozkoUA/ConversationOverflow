using ConnectToDB;
using Microsoft.EntityFrameworkCore;
using Models.Classes;
using Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Classes.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ConversationOverflowDbContext _conversationOverflowDbContext;
        public MessageRepository(ConversationOverflowDbContext conversationOverflowDbContext)
        {
            _conversationOverflowDbContext = conversationOverflowDbContext;
        }

        public async Task<Message> AddMessageAsync(Message message)
        {
            _conversationOverflowDbContext.Messages.Add(message);

            await _conversationOverflowDbContext.SaveChangesAsync();
            return message;
        }

        public async Task<Attachment> AddAttachmentAsync(Attachment attachment)
        {
            _conversationOverflowDbContext.Attachments.Add(attachment);

            await _conversationOverflowDbContext.SaveChangesAsync();
            return attachment;
        }

        public async Task<UserMessage> AddUserMessageAsync(UserMessage userMessage)
        {
            _conversationOverflowDbContext.UserMessages.Add(userMessage);

            await _conversationOverflowDbContext.SaveChangesAsync();
            return userMessage;
        }

        public async Task<bool> Send(string login, UserMessage userMessage, Message message, Attachment attachment)
        {
            if (userMessage == null) return false;

            if (IsGroup(login, userMessage.GroupId).Result)
            {
                User user = await _conversationOverflowDbContext.Users.AsQueryable()
                    .Where(user => user.Login == login)
                    .FirstOrDefaultAsync();
                if (user != null)
                {
                    userMessage.UserId = user.Id;
                    if (message != null)
                    {
                        message = await AddMessageAsync(message);
                        userMessage.MessageId = message.Id;
                    }
                    if (attachment != null)
                    {
                        attachment = await AddAttachmentAsync(attachment);
                        userMessage.AttachmentId = attachment.Id;
                    }
                    if (userMessage != null) userMessage = await AddUserMessageAsync(userMessage);

                    if (userMessage != null && (message != null || attachment != null)) return true;
                    else return false;
                }
                else return false;
            }
            else return false;
        }

        public async Task<List<MessageDto>> GetMessageAsync(string login, int groupId)
        {
            if (IsGroup(login, groupId).Result)
            {
                return await _conversationOverflowDbContext.UserMessages.AsQueryable()
                                .Include(um => um.Message)
                                .Include(um => um.User)
                                .Include(um => um.Group)
                                .Include(um => um.Attachment)
                                .Where(um => um.GroupId == groupId)
                                .Select(um => new MessageDto()
                                {
                                    Message = um.Message,
                                    Attachment = um.Attachment,
                                    UserId = um.User.Id,
                                    UserName = um.User.FirstName + " " + um.User.LastName,
                                    Login = um.User.Login,
                                    ImagePath = um.User.ImagePath
                                })
                                .OrderBy(m => m.Message.CreationTime)
                                .ToListAsync();
            }
            else return new List<MessageDto>();
        }

        public async Task<List<MessageDto>> GetMessageRangeAsync(string login, int groupId, int interval, int index)
        {
            if (IsGroup(login, groupId).Result)
            {
                return await _conversationOverflowDbContext.UserMessages.AsQueryable()
                                .Include(um => um.Message)
                                .Include(um => um.User)
                                .Include(um => um.Group)
                                .Include(um => um.Attachment)
                                .Where(um => um.GroupId == groupId)
                                .Select(um => new MessageDto()
                                {
                                    Message = um.Message,
                                    Attachment = um.Attachment,
                                    UserId = um.User.Id,
                                    UserName = um.User.FirstName + " " + um.User.LastName,
                                    Login = um.User.Login,
                                    ImagePath = um.User.ImagePath
                                })
                                .OrderByDescending(m => m.Message.CreationTime)
                                .Skip(index*interval)
                                .Take(interval)
                                .OrderBy(m => m.Message.CreationTime)
                                .ToListAsync();
            }
            else return new List<MessageDto>();
        }

        public async Task<List<MessageDto>> GetMessageRangeAtAsync(string login, int groupId, int startId, int count)
        {
            if (IsGroup(login, groupId).Result)
            {
                return await _conversationOverflowDbContext.UserMessages.AsQueryable()
                                .Include(um => um.Message)
                                .Include(um => um.User)
                                .Include(um => um.Group)
                                .Include(um => um.Attachment)
                                .Where(um => um.GroupId == groupId)
                                .Select(um => new MessageDto()
                                {
                                    Message = um.Message,
                                    Attachment = um.Attachment,
                                    UserId = um.User.Id,
                                    UserName = um.User.FirstName + " " + um.User.LastName,
                                    Login = um.User.Login,
                                    ImagePath = um.User.ImagePath
                                })
                                .OrderByDescending(m => m.Message.CreationTime)
                                .Where(m => m.Message.Id < startId)
                                .Take(count)
                                .OrderBy(m => m.Message.CreationTime)
                                .ToListAsync();
            }
            else return new List<MessageDto>();
        }

        public async Task<Attachment> GetAttachmentByMessageIdAsync(string login, int groupId, int messageId)
        {
            if (IsGroup(login, groupId).Result)
            {
                return await _conversationOverflowDbContext.UserMessages.AsQueryable()
                                .Include(um => um.Message)
                                .Include(um => um.User)
                                .Include(um => um.Group)
                                .Include(um => um.Attachment)
                                .Where(um => um.MessageId == messageId)
                                .Select(um => um.Attachment)
                                .FirstOrDefaultAsync();
            }
            else return new Attachment();
        }

        public async Task<bool> IsGroup(string login, int groupId)
            => (await _conversationOverflowDbContext.GroupUsers.AsQueryable()
            .Include(gu => gu.Group)
            .Include(gu => gu.User)
            .Where(gu => gu.User.Login == login && gu.Group.Id == groupId)
            .FirstOrDefaultAsync()) != null;
    }
}
