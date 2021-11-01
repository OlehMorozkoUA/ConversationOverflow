using System.ComponentModel.DataAnnotations;

namespace ConversationOverflow.Dto
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string ReturnUrl { get; set; }
    }
}
