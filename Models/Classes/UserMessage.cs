using Models.Interfaces;

namespace Models.Classes
{
    public class UserMessage : IUserMessage
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public int FromUserId { get; set; }
        public int ToUserOrGroupId { get; set; }
        public bool IsToUser { get; set; }
        public int AttachmentId { get; set; }

        public User ToUser { get; set; }

        public Message Message { get; set; }

        public User FromUser { get; set; }

        public Attachment Attachment { get; set; }

        public Group ToGroup { get; set; }
    }
}
