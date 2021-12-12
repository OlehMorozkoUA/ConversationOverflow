using Models.Classes;

namespace Services.Classes
{
    public class MessageDto
    {
        public Message Message { get; set; }
        public Attachment Attachment { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Login { get; set; }
        public string ImagePath { get; set; }
    }
}
