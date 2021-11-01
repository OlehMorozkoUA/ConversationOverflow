using System.ComponentModel.DataAnnotations;

namespace ConversationOverflowMVC.Dto
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
