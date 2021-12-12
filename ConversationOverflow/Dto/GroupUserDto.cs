using System.ComponentModel.DataAnnotations;

namespace ConversationOverflow.Dto
{
    public class GroupUserDto
    {
        [Required]
        public int GroupId { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}
