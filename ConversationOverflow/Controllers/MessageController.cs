using ConversationOverflow.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Classes;
using Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Services.Classes;
using System.IO;

namespace ConversationOverflow.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMessageRepository _messages;
        private readonly IGroupRepository _groups;
        private readonly IUserRepository _users;

        public MessageController(ILogger<UserController> logger,
            IMessageRepository messages,
            IGroupRepository groups,
            IUserRepository users)
        {
            _logger = logger;
            _messages = messages;
            _groups = groups;
            _users = users;
        }

        [HttpGet]
        [Route("{groupId}")]
        public async Task<List<MessageDto>> GetMessage(int groupId)
            => await _messages.GetMessageAsync(User.Identity.Name, groupId);

        [HttpGet]
        [Route("{groupId}/{interval}/{index}")]
        public async Task<List<MessageDto>> GetMessageRange(int groupId, int interval, int index = 0)
            => await _messages.GetMessageRangeAsync(User.Identity.Name, groupId, interval, index);

        [HttpGet]
        [Route("RangeAt/{groupId}/{startId}/{count}")]
        public async Task<List<MessageDto>> GetMessageRangeAt(int groupId, int startId, int count)
            => await _messages.GetMessageRangeAtAsync(User.Identity.Name, groupId, startId, count);

        [HttpGet]
        [Route("Attachment/{groupId}/{messageId}")]
        public async Task<Attachment> GetAttachmentByMessageId(int groupId, int messageId)
            => await _messages.GetAttachmentByMessageIdAsync(User.Identity.Name, groupId, messageId);

        [HttpGet]
        [Route("[action]/{groupId}")]
        public async Task<bool> IsGroup(int groupId)
            => await _messages.IsGroup(User.Identity.Name, groupId);

        [HttpPost]
        [Route("[action]")]
        public async Task<bool> Send(SendDto sendDto)
            => await _messages.Send(User.Identity.Name, sendDto.UserMessage, sendDto.Message, sendDto.Attachment);

        [HttpPost]
        public async Task<Message> Post(Message message)
            => await _messages.AddMessageAsync(message);

        [HttpPost]
        [Route("[action]")]
        public async Task<UserMessage> AddUserMessage(UserMessage userMessage)
            => await _messages.AddUserMessageAsync(userMessage);

        [HttpPost]
        [Route("Attachment")]
        public async Task UploadFile([FromForm]AttachmentDto attachmentDto)
        {
            using (var file = new FileStream(attachmentDto.FilePath, FileMode.Create))
            {
                await attachmentDto.File.CopyToAsync(file);
            }
        }
    }
}
