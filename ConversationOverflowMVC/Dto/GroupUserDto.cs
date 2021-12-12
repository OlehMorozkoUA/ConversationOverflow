using System.ComponentModel.DataAnnotations;

namespace ConversationOverflowMVC.Dto
{
    public class GroupUserDto
    {
        [Required]
        public int GroupId { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}
