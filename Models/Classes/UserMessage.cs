using Models.Interfaces;

namespace Models.Classes
{
    public class UserMessage : IUserMessage
    {
        public int Id { get; set; }
        public int? MessageId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public int? AttachmentId { get; set; }

        public Message Message { get; set; }
        public User User { get; set; }
        public Group Group { get; set; }
        public Attachment Attachment { get; set; }
    }
}
