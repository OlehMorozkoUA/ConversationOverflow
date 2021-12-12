using System.ComponentModel.DataAnnotations;

namespace Services.Classes
{
    public class UserDto
    {
        [Required]
        [Display(Name = "Login")]
        [StringLength(10)]
        public string Login { get; set; }
        [Required]
        public string PasswordHash { get; set; }
    }
}
