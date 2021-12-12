using System.ComponentModel.DataAnnotations;

namespace ConversationOverflowMVC.Dto
{
    public class UserIdDto
    {
        [Required]
        public int UserId { get; set; }
    }
}
