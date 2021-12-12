using System.ComponentModel.DataAnnotations;

namespace ConversationOverflow.Dto
{
    public class UserIdDto
    {
        [Required]
        public int UserId { get; set; }
    }
}
