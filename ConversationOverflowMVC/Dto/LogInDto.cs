using System.ComponentModel.DataAnnotations;

namespace ConversationOverflowMVC.Dto
{
    public class LogInDto
    {
        [Required]
        [Display(Name = "Login")]
        [StringLength(10)]
        public string Login { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Display(Name = "Запам'ятати")]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}
