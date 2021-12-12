using Models.Classes;
using System.ComponentModel.DataAnnotations;

namespace ConversationOverflow.Dto
{
    public class SendDto
    {
        [Required]
        public UserMessage UserMessage { get; set; }
        public Message Message { get; set; }
        public Attachment Attachment { get; set; }
    }
}
